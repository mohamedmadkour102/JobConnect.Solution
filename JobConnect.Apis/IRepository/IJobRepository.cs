using JobConnect.Apis.Models;

namespace JobConnect.Apis.IRepository
{
	public interface IJobRepository
	{
		Task<IEnumerable<Job>> GetRecentJobsAsync();
		Task<IEnumerable<Job>> GetAllJobsAsync();
		Task<Job?> GetJobByIdAsync(int jobId);
		Task AddJobAsync(Job job);
		Task UpdateJobAsync(Job job);
		Task DeleteJobAsync(int jobId);
		Task<int> GetJobsCountAsync();
		Task<int> GetCandidatesCountAsync();
	}
}
