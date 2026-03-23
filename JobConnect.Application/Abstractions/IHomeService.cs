using JobConnect.Application.DTOs;
using JobConnect.Application.DTOs.Admin;
using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions;

public interface IHomeService
{
    Task SubmitContactAsync(ContactMessageDto dto, CancellationToken cancellationToken = default);
    Task<List<ContactMessage>> GetAllMessagesAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task<object> GetJobsAsync(IReadOnlyDictionary<string, string>? filters, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<object> GetJobsByTagAndIdAsync(string tag, int tagId, CancellationToken cancellationToken = default);
}
