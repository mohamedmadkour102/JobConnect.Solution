//using JobConnect.Core.Models;
//using JobConnect.Apis.DTO_s.EmployerDto;
//using JobConnect.Apis.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace JobConnect.Apis.IService
//{
//	public interface IEmployerService
//	{
//		Task<Employer> GetEmployerByIdAsync(string employerId);
//		Task UpdateCompanyInfoAsync(string employerId, UpdateCompanyInfoDto dto);
//		Task UpdateFoundingInfoAsync(string employerId, UpdateFoundingInfoDto dto);
//		Task ChangePasswordAsync(string employerId, ChangePasswordDto dto);
//		Task<IEnumerable<JobDto>> GetRecentJobsAsync(string employerId);
//		Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId);
//		Task<JobDto?> GetJobByIdAsync(int jobId, string employerId);
//		Task AddJobAsync(string employerId, CreateJobDto jobDto);
//		Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto);
//		Task DeleteJobAsync(int jobId, string employerId);
//		Task<JobStatsDto> GetJobStatsAsync(string employerId);
//		Task AddToShortlistAsync(int jobId, string jobSeekerId);
//		Task RemoveFromShortlistAsync(int jobId, string jobSeekerId);
//		Task<IEnumerable<ShortlistedJobSeekerDto>> GetShortlistedJobSeekersAsync(int jobId, string employerId);
//		Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize);
//        Task DeleteEmployerAccountAsync(string employerId);
//        Task<ApplicantDto?> GetJobSeekerByIdAsync(string employerId, string jobSeekerId);
//    }
//}
using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.IService
{
    public interface IEmployerService
    {
        Task<Employer> GetEmployerByIdAsync(string employerId);
        Task UpdateCompanyInfoAsync(string employerId, UpdateCompanyInfoDto dto);
        Task UpdateFoundingInfoAsync(string employerId, UpdateFoundingInfoDto dto);
        Task ChangePasswordAsync(string employerId, ChangePasswordDto dto);
        Task<IEnumerable<DTO_s.EmployerDto.JobDto>> GetRecentJobsAsync(string employerId);
        Task<IEnumerable<DTO_s.EmployerDto.JobDto>> GetJobsByEmployerAsync(string employerId);
        Task<DTO_s.EmployerDto.JobDto?> GetJobByIdAsync(int jobId, string employerId);
        Task AddJobAsync(string employerId, CreateJobDto jobDto);
        Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto);
        Task DeleteJobAsync(int jobId, string employerId);
        Task<JobStatsDto> GetJobStatsAsync(string employerId);
        Task AddToShortlistAsync(int jobId, string jobSeekerId);
        Task RemoveFromShortlistAsync(int jobId, string jobSeekerId);
        Task<IEnumerable<ShortlistedJobSeekerDto>> GetShortlistedJobSeekersAsync(int jobId, string employerId);
        Task<(IEnumerable<DTO_s.EmployerDto.JobDto> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize);
        Task DeleteEmployerAccountAsync(string employerId);
        Task<SeekerProfileDto?> GetJobSeekerByIdAsync(string employerId, string jobSeekerId);
        Task<IEnumerable<ResumeDto>> GetSeekerResumesWithIdAsync(string jobSeekerId);
        Task<IEnumerable<JobApplicantWithResumeDto>> GetApplicantsWithResumeAsync(int jobId, string employerId);
    }
}