using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class AdminRepository : RepositoryBase<User>, IAdminRepository
{
    public AdminRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Employer>> GetAllEmployersAsync(CancellationToken cancellationToken = default) =>
        await Context.Employers
            .Include(e => e.Jobs)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync(CancellationToken cancellationToken = default) =>
        await Context.JobSeekers
            .Include(js => js.Applications)
            .Include(js => js.SavedJobs)
            .Include(js => js.Resumes)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasActiveEmployerJobsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var employer = await Context.Employers
            .Include(e => e.Jobs)
            .FirstOrDefaultAsync(e => e.Id == userId, cancellationToken);

        return employer != null && employer.Jobs.Any();
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await Context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
            return false;

        Context.Users.Remove(user);
        return true;
    }

    public async Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default) =>
        await Context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Applications)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Job>> GetJobsByTagAsync(string tag, CancellationToken cancellationToken = default) =>
        await Context.Jobs
            .Include(j => j.Employer)
            .Include(j => j.Tags)
            .Include(j => j.Responsibilities)
            .Include(j => j.Applications)
            .Where(j => j.Tags.Any(t => t.Tag == tag))
            .ToListAsync(cancellationToken);
}
