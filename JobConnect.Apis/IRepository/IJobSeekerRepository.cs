using JobConnect.Apis.Models;
using JobConnect.Core.Models;

namespace JobConnect.Apis.IRepository
{
	public interface IJobSeekerRepository
	{
		Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
		Task UpdateJobSeekerAsync(JobSeeker jobSeeker);
		Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId);
		Task SaveJobAsync(string jobSeekerId, int jobId);
		Task UnsaveJobAsync(string jobSeekerId, int jobId);
		Task<IEnumerable<Job>> GetAllJobsAsync();
		Task<Job> GetJobByIdAsync(int jobId);
		Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath);
		Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId);
		Task<IEnumerable<Employer>> GetAllEmployersAsync(); // New method
		Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);


	}
}
