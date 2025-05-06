using JobConnect.Apis.Models;
using JobConnect.Core.Models;

namespace JobConnect.Apis.IRepository
{
	public interface IAdminRepository
	{
		Task<IEnumerable<Employer>> GetAllEmployersAsync();
		Task<IEnumerable<JobSeeker>> GetAllJobSeekersAsync();
		Task<bool> DeleteUserAsync(string userId);
		Task<IEnumerable<Job>> GetAllJobsAsync();
		Task<IEnumerable<Job>> GetJobsByTagAsync(string tag);
	}
}
