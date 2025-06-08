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
    }
}