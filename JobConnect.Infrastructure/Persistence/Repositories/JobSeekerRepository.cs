using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Application.DTOs.SeekerDto;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class JobSeekerRepository : RepositoryBase<JobSeeker>, IJobSeekerRepository
{
    public JobSeekerRepository(AppDbContext context) : base(context) { }

    public async Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(jobSeekerId))
            return null;

        return await Context.JobSeekers
            .Include(js => js.SavedJobs)
            .ThenInclude(sj => sj.Job)
            .ThenInclude(j => j!.Employer)
            .Include(js => js.Applications)
            .ThenInclude(a => a.Job)
            .Include(js => js.Resumes)
            .FirstOrDefaultAsync(js => js.Id == jobSeekerId, cancellationToken);
    }

    public async Task<JobSeeker?> GetSeekerProfileAsync(string jobSeekerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(jobSeekerId))
            return null;

        return await Context.JobSeekers
            .Include(js => js.Resumes)
            .Include(js => js.Certifications)
            .Include(js => js.CompanyWorkedAt)
            .Include(js => js.Skills)
            .Include(js => js.WorkedAs)
            .FirstOrDefaultAsync(js => js.Id == jobSeekerId, cancellationToken);
    }

    public void UpdateJobSeeker(JobSeeker jobSeeker) => Context.JobSeekers.Update(jobSeeker);

    public async Task DeleteJobSeekerAsync(string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId, cancellationToken);
        if (jobSeeker != null)
            Context.JobSeekers.Remove(jobSeeker);
    }

    public async Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId, CancellationToken cancellationToken = default) =>
        await Context.SavedJobs
            .Where(sj => sj.JobSeekerId == jobSeekerId)
            .Include(sj => sj.Job)
            .ThenInclude(j => j!.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(sj => sj.Job)
            .ThenInclude(j => j!.Tags)
            .Include(sj => sj.Job)
            .ThenInclude(j => j!.Responsibilities)
            .Include(sj => sj.Job)
            .ThenInclude(j => j!.Employer)
            .Select(sj => sj.Job!)
            .ToListAsync(cancellationToken);

    public async Task SaveJobAsync(string jobSeekerId, int jobId, CancellationToken cancellationToken = default)
    {
        var existing = await Context.SavedJobs
            .FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("You have already saved this job.");

        await Context.SavedJobs.AddAsync(new SavedJob
        {
            JobSeekerId = jobSeekerId,
            JobId = jobId,
            SavedDate = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task UnsaveJobAsync(string jobSeekerId, int jobId, CancellationToken cancellationToken = default)
    {
        var existing = await Context.SavedJobs
            .FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId, cancellationToken);
        if (existing != null)
            Context.SavedJobs.Remove(existing);
    }

    public async Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default) =>
        await Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Employer)
            .ToListAsync(cancellationToken);

    public async Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default) =>
        await Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Employer)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

    public async Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath, CancellationToken cancellationToken = default)
    {
        await Context.Applications.AddAsync(new JobApplication
        {
            JobSeekerId = jobSeekerId,
            JobId = jobId,
            CoverLetter = coverLetter,
            Resume = resumePath,
            ApplicationDate = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId, CancellationToken cancellationToken = default) =>
        await Context.Applications
            .Where(a => a.JobSeekerId == jobSeekerId)
            .Include(a => a.Job)
            .ThenInclude(j => j!.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(a => a.Job)
            .ThenInclude(j => j!.Tags)
            .Include(a => a.Job)
            .ThenInclude(j => j!.Responsibilities)
            .Include(a => a.Job)
            .ThenInclude(j => j!.Employer)
            .Include(a => a.JobSeeker)
            .Select(a => a.Job!)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Employer);

        var totalCount = await query.CountAsync(cancellationToken);
        var jobs = await query
            .OrderBy(j => j.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (jobs, totalCount);
    }

    public async Task<IEnumerable<Employer>> GetAllEmployersAsync(CancellationToken cancellationToken = default) =>
        await Context.Employers
            .Include(e => e.Jobs)
            .ToListAsync(cancellationToken);

    public async Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto, CancellationToken cancellationToken = default)
    {
        var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId, cancellationToken)
            ?? throw new InvalidOperationException("JobSeeker not found.");

        var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == applyDto.ResumeId)
            ?? throw new InvalidOperationException("Selected resume not found in your profile.");

        var existingApplication = await Context.Applications
            .AnyAsync(a => a.JobSeekerId == jobSeekerId && a.JobId == applyDto.JobId, cancellationToken);
        if (existingApplication)
            throw new InvalidOperationException("You have already applied for this job.");

        await Context.Applications.AddAsync(new JobApplication
        {
            JobSeekerId = jobSeekerId,
            JobId = applyDto.JobId,
            Resume = resume.ResumePath ?? string.Empty,
            ApplicationDate = DateTime.UtcNow,
            CoverLetter = applyDto.CoverLetter,
        }, cancellationToken);
    }

    public async Task<ProfileCompletionDto?> GetProfileCompletionAsync(string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var jobSeeker = await GetSeekerProfileAsync(jobSeekerId, cancellationToken);
        if (jobSeeker == null) return null;

        const int totalFields = 22;
        var completedFields = 11;

        var fieldDetails = new List<FieldStatus>
        {
            new() { FieldName = "Address", Status = string.IsNullOrWhiteSpace(jobSeeker.Address) || jobSeeker.Address == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Address },
            new() { FieldName = "YearsOfExperience", Status = jobSeeker.YearsOfExperience.HasValue ? "Completed" : "Not Completed", Value = jobSeeker.YearsOfExperience?.ToString() },
            new() { FieldName = "Degree", Status = string.IsNullOrWhiteSpace(jobSeeker.Degree) || jobSeeker.Degree == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Degree },
            new() { FieldName = "CurrentOrDesiredJob", Status = string.IsNullOrWhiteSpace(jobSeeker.CurrentOrDesiredJob) || jobSeeker.CurrentOrDesiredJob == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CurrentOrDesiredJob },
            new() { FieldName = "Bio", Status = string.IsNullOrWhiteSpace(jobSeeker.Bio) || jobSeeker.Bio == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Bio },
            new() { FieldName = "CoverLetter", Status = string.IsNullOrWhiteSpace(jobSeeker.CoverLetter) || jobSeeker.CoverLetter == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CoverLetter },
            new() { FieldName = "DateOfBirth", Status = jobSeeker.DateOfBirth.HasValue ? "Completed" : "Not Completed", Value = jobSeeker.DateOfBirth?.ToString() },
            new() { FieldName = "Nationality", Status = string.IsNullOrWhiteSpace(jobSeeker.Nationality) || jobSeeker.Nationality == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Nationality },
            new() { FieldName = "MaritalStatus", Status = string.IsNullOrWhiteSpace(jobSeeker.MaritalStatus) || jobSeeker.MaritalStatus == "string" ? "Not Completed" : "Completed", Value = jobSeeker.MaritalStatus },
            new() { FieldName = "Gender", Status = string.IsNullOrWhiteSpace(jobSeeker.Gender) || jobSeeker.Gender == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Gender },
            new() { FieldName = "Education", Status = string.IsNullOrWhiteSpace(jobSeeker.Education) || jobSeeker.Education == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Education },
            new() { FieldName = "Portfolio", Status = string.IsNullOrWhiteSpace(jobSeeker.Portfolio) || jobSeeker.Portfolio == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Portfolio },
            new() { FieldName = "FacebookLink", Status = string.IsNullOrWhiteSpace(jobSeeker.FacebookLink) || jobSeeker.FacebookLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.FacebookLink },
            new() { FieldName = "TwitterLink", Status = string.IsNullOrWhiteSpace(jobSeeker.TwitterLink) || jobSeeker.TwitterLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.TwitterLink },
            new() { FieldName = "InstagramLink", Status = string.IsNullOrWhiteSpace(jobSeeker.InstagramLink) || jobSeeker.InstagramLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.InstagramLink },
            new() { FieldName = "LinkedInLink", Status = string.IsNullOrWhiteSpace(jobSeeker.LinkedInLink) || jobSeeker.LinkedInLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.LinkedInLink },
            new() { FieldName = "CollegeName", Status = string.IsNullOrWhiteSpace(jobSeeker.CollegeName) || jobSeeker.CollegeName == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CollegeName },
            new() { FieldName = "University", Status = string.IsNullOrWhiteSpace(jobSeeker.University) || jobSeeker.University == "string" ? "Not Completed" : "Completed", Value = jobSeeker.University }
        };

        completedFields += fieldDetails.Count(f => f.Status == "Completed");

        var completionPercentage = (double)completedFields / totalFields * 100;

        return new ProfileCompletionDto
        {
            TotalFields = totalFields,
            CompletedFields = completedFields,
            CompletionPercentage = Math.Round(completionPercentage, 2),
            FieldDetails = fieldDetails
        };
    }

    public async Task<Employer?> GetEmployerByIdAsync(string employerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(employerId))
            return null;

        return await Context.Employers
            .Include(e => e.Jobs)
            .ThenInclude(j => j.Tags)
            .Include(e => e.Jobs)
            .ThenInclude(j => j.Responsibilities)
            .FirstOrDefaultAsync(e => e.Id == employerId, cancellationToken);
    }

    public async Task UploadResumeAsync(string jobSeekerId, UploadResumeDto uploadDto, CancellationToken cancellationToken = default)
    {
        var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId, cancellationToken)
            ?? throw new InvalidOperationException("JobSeeker not found.");

        var newResume = new JobSeekerResume
        {
            JobSeekerId = jobSeekerId,
            ResumePath = string.Empty,
            ResumeName = uploadDto.Resume.FileName,
            UploadDate = DateTime.UtcNow
        };

        jobSeeker.Resumes.Add(newResume);
    }

    public async Task DeleteResumeAsync(string jobSeekerId, int resumeId, CancellationToken cancellationToken = default)
    {
        var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId, cancellationToken)
            ?? throw new InvalidOperationException("JobSeeker not found.");

        var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == resumeId)
            ?? throw new InvalidOperationException("Resume not found.");

        Context.JobSeekerResumes.Remove(resume);
    }

    public async Task<IEnumerable<ResumeInfoDto>> GetResumesAsync(string jobSeekerId, CancellationToken cancellationToken = default) =>
        await Context.JobSeekerResumes
            .Where(r => r.JobSeekerId == jobSeekerId)
            .Select(r => new ResumeInfoDto
            {
                Id = r.Id,
                ResumeName = r.ResumeName,
                ResumePath = r.ResumePath
            })
            .ToListAsync(cancellationToken);

    public async Task<List<JobSeeker>> GetJobSeekersForMatchingAsync(CancellationToken cancellationToken = default) =>
        await Context.JobSeekers
            .Include(js => js.Skills)
            .Include(js => js.WorkedAs)
            .ToListAsync(cancellationToken);
}
