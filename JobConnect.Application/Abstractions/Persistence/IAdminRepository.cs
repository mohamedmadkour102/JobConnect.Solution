using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions.Persistence;

public interface IAdminRepository : IRepository<User>
{
    Task<IEnumerable<Employer>> GetAllEmployersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync(CancellationToken cancellationToken = default);
    Task<bool> HasActiveEmployerJobsAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetJobsByTagAsync(string tag, CancellationToken cancellationToken = default);
}
