using JobConnect.Application.DTOs.EmployerDto;
using JobConnect.Application.DTOs.SeekerDto;
using JobConnect.Domain.Entities;
using EmployerJobDto = JobConnect.Application.DTOs.EmployerDto.JobDto;

namespace JobConnect.Application.Abstractions;

public interface IEmployerService
{
    Task<Employer> GetEmployerByIdAsync(string employerId);
    Task UpdateCompanyInfoAsync(string employerId, UpdateCompanyInfoDto dto);
    Task UpdateFoundingInfoAsync(string employerId, UpdateFoundingInfoDto dto);
    Task ChangePasswordAsync(string employerId, ChangePasswordDto dto);
    Task<IEnumerable<EmployerJobDto>> GetRecentJobsAsync(string employerId);
    Task<IEnumerable<EmployerJobDto>> GetJobsByEmployerAsync(string employerId);
    Task<EmployerJobDto?> GetJobByIdAsync(int jobId, string employerId);
    Task AddJobAsync(string employerId, CreateJobDto jobDto);
    Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto);
    Task DeleteJobAsync(int jobId, string employerId);
    Task<JobStatsDto> GetJobStatsAsync(string employerId);
    Task AddToShortlistAsync(int jobId, string jobSeekerId);
    Task RemoveFromShortlistAsync(int jobId, string jobSeekerId);
    Task<IEnumerable<ShortlistedJobSeekerDto>> GetShortlistedJobSeekersAsync(int jobId, string employerId);
    Task<(IEnumerable<EmployerJobDto> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize);
    Task DeleteEmployerAccountAsync(string employerId);
    Task<SeekerProfileDto?> GetJobSeekerByIdAsync(string employerId, string jobSeekerId);
    Task<IEnumerable<ResumeDto>> GetSeekerResumesWithIdAsync(string jobSeekerId);
    Task<IEnumerable<JobApplicantWithResumeDto>> GetApplicantsWithResumeAsync(int jobId, string employerId);
    Task<bool> HireApplicantAsync(string employerId, int jobId, string jobSeekerId);
    Task<bool> RejectApplicantAsync(string employerId, int jobId, string jobSeekerId);
}
