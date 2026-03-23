using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class EmployerRepository : RepositoryBase<Employer>, IEmployerRepository
{
    public EmployerRepository(AppDbContext context) : base(context) { }

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

    public void UpdateEmployer(Employer employer) => Context.Employers.Update(employer);

    public async Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId, CancellationToken cancellationToken = default)
    {
        return await Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Where(j => j.EmployerId == employerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId, CancellationToken cancellationToken = default)
    {
        return await Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Where(j => j.EmployerId == employerId)
            .OrderByDescending(j => j.PostedDate)
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<Job?> GetJobByIdAsync(int jobId, string employerId, CancellationToken cancellationToken = default)
    {
        return await Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId, cancellationToken);
    }

    public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(
        string employerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = Context.Jobs
            .Include(j => j.Applications)
            .ThenInclude(a => a.JobSeeker)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Where(j => j.EmployerId == employerId);

        var totalCount = await query.CountAsync(cancellationToken);
        var jobs = await query
            .OrderBy(j => j.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (jobs, totalCount);
    }

    public async Task AddJobAsync(Job job, CancellationToken cancellationToken = default) =>
        await Context.Jobs.AddAsync(job, cancellationToken);

    public void UpdateJob(Job job) => Context.Jobs.Update(job);

    public async Task DeleteJobAsync(int jobId, string employerId, CancellationToken cancellationToken = default)
    {
        var job = await Context.Jobs
            .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId, cancellationToken);
        if (job != null)
            Context.Jobs.Remove(job);
    }

    public async Task<int> GetJobsCountAsync(string employerId, CancellationToken cancellationToken = default) =>
        await Context.Jobs.CountAsync(j => j.EmployerId == employerId, cancellationToken);

    public async Task<int> GetCandidatesCountAsync(string employerId, CancellationToken cancellationToken = default) =>
        await Context.Applications
            .Where(a => a.Job.EmployerId == employerId)
            .Select(a => a.JobSeekerId)
            .Distinct()
            .CountAsync(cancellationToken);

    public async Task AddToShortlistAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var application = await Context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId, cancellationToken)
            ?? throw new InvalidOperationException("Application not found.");
        application.IsShortlisted = true;
    }

    public async Task RemoveFromShortlistAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var application = await Context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId, cancellationToken)
            ?? throw new InvalidOperationException("Application not found.");
        application.IsShortlisted = false;
    }

    public async Task<IEnumerable<JobApplication>> GetShortlistedJobSeekersAsync(int jobId, string employerId, CancellationToken cancellationToken = default) =>
        await Context.Applications
            .Where(a => a.JobId == jobId && a.Job.EmployerId == employerId && a.IsShortlisted)
            .Include(a => a.JobSeeker)
            .ToListAsync(cancellationToken);

    public async Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<JobApplication>> GetApplicationsByJobAsync(int jobId, CancellationToken cancellationToken = default) =>
        await Context.Applications
            .Include(a => a.JobSeeker)
            .Where(a => a.JobId == jobId)
            .ToListAsync(cancellationToken);

    public async Task<bool> HireApplicantAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var application = await Context.Applications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId && a.Status == "Pending", cancellationToken);
        if (application?.Job == null)
            return false;

        var job = application.Job;
        if (job.Vacancies <= 0)
            return false;

        application.Status = "Accepted";
        job.Vacancies -= 1;
        job.ApplicationCount = await Context.Applications.CountAsync(
            a => a.JobId == jobId && a.Status == "Accepted", cancellationToken);
        return true;
    }

    public async Task<bool> RejectApplicantAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default)
    {
        var application = await Context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.JobSeekerId == jobSeekerId && a.Status == "Pending", cancellationToken);
        if (application == null)
            return false;
        application.Status = "Rejected";
        return true;
    }
}
