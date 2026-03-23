using JobConnect.Application.DTOs.Admin;
namespace JobConnect.Application.Abstractions;

public interface IAdminService
{
    Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
    Task<IEnumerable<JobSeekerDto>> GetAllJobSeekersAsync();
    Task<bool> DeleteUserAsync(string userId);
    Task<IEnumerable<JobDto>> GetAllJobsAsync();
    Task<IEnumerable<JobDto>> GetJobsByTagAsync(string tag);
    Task<JobConnect.Domain.Entities.JobApplication?> UpdateApplicationStatusAsync(string applicationId, string status);
}
