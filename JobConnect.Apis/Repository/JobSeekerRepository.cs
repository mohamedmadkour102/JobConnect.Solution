using JobConnect.Apis.IRepository;
using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using JobConnect.Apis.DTO_s.SeekerDto;


namespace JobConnect.Apis.Repository
{
    public class JobSeekerRepository : IJobSeekerRepository
    {
        private readonly AppDbContext _context;

        public JobSeekerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId)
        {
            if (string.IsNullOrEmpty(jobSeekerId))
                return null;

            return await _context.JobSeekers
                .Include(js => js.SavedJobs)
                    .ThenInclude(sj => sj.Job)
                    .ThenInclude(j => j.Employer)
                .Include(js => js.Applications)
                    .ThenInclude(a => a.Job)
                .Include(js => js.Resumes)
                .FirstOrDefaultAsync(js => js.Id == jobSeekerId);
        }

        public async Task<JobSeeker> GetSeekerProfileAsync(string jobSeekerId)
        {
            if (string.IsNullOrEmpty(jobSeekerId))
                return null;

            return await _context.JobSeekers
                .Include(js => js.Resumes)
                .Include(js => js.Certifications)
                .Include(js => js.CompanyWorkedAt)
                .Include(js => js.Skills)
                .Include(js => js.WorkedAs)
                .FirstOrDefaultAsync(js => js.Id == jobSeekerId);
        }

        public async Task UpdateJobSeekerAsync(JobSeeker jobSeeker)
        {
            _context.JobSeekers.Update(jobSeeker);
            await SaveChangesAsync();
        }

        public async Task DeleteJobSeekerAsync(string jobSeekerId)
        {
            var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker != null)
            {
                _context.JobSeekers.Remove(jobSeeker);
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId)
        {
            return await _context.SavedJobs
                .Where(sj => sj.JobSeekerId == jobSeekerId)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Applications)
                    .ThenInclude(a => a.JobSeeker)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Tags)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Responsibilities)
                .Include(sj => sj.Job)
                    .ThenInclude(j => j.Employer)
                .Select(sj => sj.Job)
                .ToListAsync();
        }

        public async Task SaveJobAsync(string jobSeekerId, int jobId)
        {
            var existing = await _context.SavedJobs
                .FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);

            if (existing != null)
                throw new InvalidOperationException("You have already saved this job.");

            _context.SavedJobs.Add(new SavedJob
            {
                JobSeekerId = jobSeekerId,
                JobId = jobId,
                SavedDate = DateTime.UtcNow
            });

            await SaveChangesAsync();
        }

        public async Task UnsaveJobAsync(string jobSeekerId, int jobId)
        {
            var existing = await _context.SavedJobs
                .FirstOrDefaultAsync(sj => sj.JobSeekerId == jobSeekerId && sj.JobId == jobId);

            if (existing != null)
            {
                _context.SavedJobs.Remove(existing);
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                    .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Include(j => j.Employer)
                .ToListAsync();
        }

        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                    .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath)
        {
            var application = new Application
            {
                JobSeekerId = jobSeekerId,
                JobId = jobId,
                CoverLetter = coverLetter,
                Resume = resumePath,
                ApplicationDate = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(application);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId)
        {
            return await _context.Applications
                .Where(a => a.JobSeekerId == jobSeekerId)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Applications)
                    .ThenInclude(a => a.JobSeeker)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Tags)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Responsibilities)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Employer)
                .Include(a => a.JobSeeker)
                .Select(a => a.Job)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Jobs
                .Include(j => j.Applications)
                    .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Include(j => j.Employer);

            var totalCount = await query.CountAsync();

            var jobs = await query
                .OrderBy(j => j.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (jobs, totalCount);
        }

        public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
        {
            return await _context.Employers
                .Include(e => e.Jobs)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto)
        {
            var jobSeeker = await GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == applyDto.ResumeId);
            if (resume == null)
                throw new Exception("Selected resume not found in your profile.");

            var application = new Application
            {
                JobSeekerId = jobSeekerId,
                JobId = applyDto.JobId,
                Resume = resume.ResumePath,
                ApplicationDate = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(application);
            await SaveChangesAsync();
        }


        public async Task<ProfileCompletionDto> GetProfileCompletionAsync(string jobSeekerId)
        {
            var jobSeeker = await GetSeekerProfileAsync(jobSeekerId);
            if (jobSeeker == null) return null;

            var totalFields = 22; 
            var completedFields = 11;

            var fieldDetails = new List<FieldStatus>
        {
            new FieldStatus { FieldName = "Address", Status = string.IsNullOrWhiteSpace(jobSeeker.Address) || jobSeeker.Address == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Address },
            new FieldStatus { FieldName = "YearsOfExperience", Status = jobSeeker.YearsOfExperience.HasValue ? "Completed" : "Not Completed", Value = jobSeeker.YearsOfExperience?.ToString() },
            new FieldStatus { FieldName = "Degree", Status = string.IsNullOrWhiteSpace(jobSeeker.Degree) || jobSeeker.Degree == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Degree },
            new FieldStatus { FieldName = "CurrentOrDesiredJob", Status = string.IsNullOrWhiteSpace(jobSeeker.CurrentOrDesiredJob) || jobSeeker.CurrentOrDesiredJob == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CurrentOrDesiredJob },
            new FieldStatus { FieldName = "Bio", Status = string.IsNullOrWhiteSpace(jobSeeker.Bio) || jobSeeker.Bio == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Bio },
            new FieldStatus { FieldName = "CoverLetter", Status = string.IsNullOrWhiteSpace(jobSeeker.CoverLetter) || jobSeeker.CoverLetter == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CoverLetter },
            new FieldStatus { FieldName = "DateOfBirth", Status = jobSeeker.DateOfBirth.HasValue ? "Completed" : "Not Completed", Value = jobSeeker.DateOfBirth?.ToString() },
            new FieldStatus { FieldName = "Nationality", Status = string.IsNullOrWhiteSpace(jobSeeker.Nationality) || jobSeeker.Nationality == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Nationality },
            new FieldStatus { FieldName = "MaritalStatus", Status = string.IsNullOrWhiteSpace(jobSeeker.MaritalStatus) || jobSeeker.MaritalStatus == "string" ? "Not Completed" : "Completed", Value = jobSeeker.MaritalStatus },
            new FieldStatus { FieldName = "Gender", Status = string.IsNullOrWhiteSpace(jobSeeker.Gender) || jobSeeker.Gender == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Gender },
            new FieldStatus { FieldName = "Education", Status = string.IsNullOrWhiteSpace(jobSeeker.Education) || jobSeeker.Education == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Education },
            new FieldStatus { FieldName = "Portfolio", Status = string.IsNullOrWhiteSpace(jobSeeker.Portfolio) || jobSeeker.Portfolio == "string" ? "Not Completed" : "Completed", Value = jobSeeker.Portfolio },
            new FieldStatus { FieldName = "FacebookLink", Status = string.IsNullOrWhiteSpace(jobSeeker.FacebookLink) || jobSeeker.FacebookLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.FacebookLink },
            new FieldStatus { FieldName = "TwitterLink", Status = string.IsNullOrWhiteSpace(jobSeeker.TwitterLink) || jobSeeker.TwitterLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.TwitterLink },
            new FieldStatus { FieldName = "InstagramLink", Status = string.IsNullOrWhiteSpace(jobSeeker.InstagramLink) || jobSeeker.InstagramLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.InstagramLink },
            new FieldStatus { FieldName = "LinkedInLink", Status = string.IsNullOrWhiteSpace(jobSeeker.LinkedInLink) || jobSeeker.LinkedInLink == "string" ? "Not Completed" : "Completed", Value = jobSeeker.LinkedInLink },
            new FieldStatus { FieldName = "CollegeName", Status = string.IsNullOrWhiteSpace(jobSeeker.CollegeName) || jobSeeker.CollegeName == "string" ? "Not Completed" : "Completed", Value = jobSeeker.CollegeName },
            new FieldStatus { FieldName = "University", Status = string.IsNullOrWhiteSpace(jobSeeker.University) || jobSeeker.University == "string" ? "Not Completed" : "Completed", Value = jobSeeker.University }
        };

            completedFields += fieldDetails.Count(f => f.Status == "Complete");

            var completionPercentage = (double)completedFields / totalFields * 100;

            return new ProfileCompletionDto
            {
                TotalFields = totalFields,
                CompletedFields = completedFields,
                CompletionPercentage = Math.Round(completionPercentage, 2),
                FieldDetails = fieldDetails
            };
        }
        public async Task<Employer> GetEmployerByIdAsync(string employerId)
        {
            if (string.IsNullOrEmpty(employerId))
                return null;

            return await _context.Employers
                .Include(e => e.Jobs)
                    .ThenInclude(j => j.Tags)
                .Include(e => e.Jobs)
                    .ThenInclude(j => j.Responsibilities)
                .FirstOrDefaultAsync(e => e.Id == employerId);
        }

    }
}