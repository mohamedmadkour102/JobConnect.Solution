using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class ApplicationReadRepository : RepositoryBase<JobApplication>, IApplicationReadRepository
{
    public ApplicationReadRepository(AppDbContext context) : base(context) { }

    public async Task<JobApplication?> GetByIdWithJobSeekerAsync(int applicationId, CancellationToken cancellationToken = default) =>
        await Context.Applications
            .Include(a => a.JobSeeker)
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);
}
