using JobConnect.Core.Models;
using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.Models;
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
		Task<IEnumerable<JobDto>> GetRecentJobsAsync(string employerId);
		Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId);
		Task<JobDto?> GetJobByIdAsync(int jobId, string employerId);
		Task AddJobAsync(string employerId, CreateJobDto jobDto);
		Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto);
		Task DeleteJobAsync(int jobId, string employerId);
		Task<JobStatsDto> GetJobStatsAsync(string employerId);
	}
}
