using JobConnect.Apis.DTO_s.Admin;

namespace JobConnect.Apis.IService
{
	public interface IAdminService
	{
		Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
		Task<IEnumerable<JobSeekerDto>> GetAllJobSeekersAsync();
		Task<bool> DeleteUserAsync(string userId);
		Task<IEnumerable<JobDto>> GetAllJobsAsync();
		Task<IEnumerable<JobDto>> GetJobsByTagAsync(string tag);
	}
}
