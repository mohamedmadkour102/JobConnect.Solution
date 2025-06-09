using JobConnect.Apis.IRepository;
using JobConnect.Core.Models;
using JobConnect.Apis.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.Repository
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly AppDbContext _context;

        public EmployerRepository(AppDbContext context)
        {
            _context = context;
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

        public async Task UpdateEmployerAsync(Employer employer)
        {
            _context.Employers.Update(employer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Where(j => j.EmployerId == employerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Where(j => j.EmployerId == employerId)
                .OrderByDescending(j => j.PostedDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<Job?> GetJobByIdAsync(int jobId, string employerId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
        }

        public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize)
        {
            var query = _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .Include(j => j.Tags)
                .Include(j => j.Responsibilities)
                .Where(j => j.EmployerId == employerId);

            var totalCount = await query.CountAsync();

            var jobs = await query
                .OrderBy(j => j.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (jobs, totalCount);
        }

        public async Task AddJobAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJobAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJobAsync(int jobId, string employerId)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetJobsCountAsync(string employerId)
        {
            return await _context.Jobs
                .CountAsync(j => j.EmployerId == employerId);
        }

        public async Task<int> GetCandidatesCountAsync(string employerId)
        {
            return await _context.Applications
                .CountAsync(a => a.Job.EmployerId == employerId);
        }

        public async Task AddToShortlistAsync(int jobId, string jobSeekerId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId);

            if (application == null)
                throw new Exception("Application not found.");

            application.IsShortlisted = true;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromShortlistAsync(int jobId, string jobSeekerId)
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId);

            if (application == null)
                throw new Exception("Application not found.");

            application.IsShortlisted = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Application>> GetShortlistedJobSeekersAsync(int jobId, string employerId)
        {
            return await _context.Applications
                .Where(a => a.JobId == jobId && a.Job.EmployerId == employerId && a.IsShortlisted)
                .Include(a => a.JobSeeker)
                .ToListAsync();
        }

        public async Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId)
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
 

        public async Task<IEnumerable<Application>> GetApplicationsByJobAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.JobSeeker)
                .Where(a => a.JobId == jobId)
                .ToListAsync();
        }

    }
}