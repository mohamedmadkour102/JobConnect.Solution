using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions.Persistence;

public interface IHomeRepository : IRepository<ContactMessage>
{
    Task AddContactMessageAsync(ContactMessage message, CancellationToken cancellationToken = default);
    Task<List<ContactMessage>> GetContactMessagesOrderedAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetDistinctJobTagsAsync(CancellationToken cancellationToken = default);
    Task<(List<Job> Jobs, int TotalCount)> GetJobsWithFiltersAsync(
        IReadOnlyDictionary<string, string>? filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<List<Job>> GetJobsByTagAndIdAsync(string tag, int tagId, CancellationToken cancellationToken = default);
}
