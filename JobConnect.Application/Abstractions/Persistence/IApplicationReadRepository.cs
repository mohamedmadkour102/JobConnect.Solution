namespace JobConnect.Application.Abstractions.Persistence;

public interface IApplicationReadRepository
{
    Task<JobConnect.Domain.Entities.JobApplication?> GetByIdWithJobSeekerAsync(int applicationId, CancellationToken cancellationToken = default);
}
