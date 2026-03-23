using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions.Persistence;

public interface IEmployerRepository : IRepository<Employer>
{
    Task<Employer?> GetEmployerByIdAsync(string employerId, CancellationToken cancellationToken = default);
    void UpdateEmployer(Employer employer);
    Task<IEnumerable<Job>> GetJobsByEmployerAsync(string employerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetRecentJobsByEmployerAsync(string employerId, CancellationToken cancellationToken = default);
    Task<Job?> GetJobByIdAsync(int jobId, string employerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Job> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddJobAsync(Job job, CancellationToken cancellationToken = default);
    void UpdateJob(Job job);
    Task DeleteJobAsync(int jobId, string employerId, CancellationToken cancellationToken = default);
    Task<int> GetJobsCountAsync(string employerId, CancellationToken cancellationToken = default);
    Task<int> GetCandidatesCountAsync(string employerId, CancellationToken cancellationToken = default);
    Task AddToShortlistAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default);
    Task RemoveFromShortlistAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JobConnect.Domain.Entities.JobApplication>> GetShortlistedJobSeekersAsync(int jobId, string employerId, CancellationToken cancellationToken = default);
    Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<JobConnect.Domain.Entities.JobApplication>> GetApplicationsByJobAsync(int jobId, CancellationToken cancellationToken = default);
    Task<bool> HireApplicantAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default);
    Task<bool> RejectApplicantAsync(int jobId, string jobSeekerId, CancellationToken cancellationToken = default);
}
