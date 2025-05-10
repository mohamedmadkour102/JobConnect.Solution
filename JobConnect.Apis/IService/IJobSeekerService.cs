using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Core.Models;

namespace JobConnect.Apis.IService
{
	public interface IJobSeekerService
	{
		Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
		Task<IEnumerable<SavedJobDto>> GetSavedJobsAsync(string jobSeekerId);
		Task SaveJobAsync(string jobSeekerId, int jobId);
		Task UnsaveJobAsync(string jobSeekerId, int jobId);
		Task<IEnumerable<JobDto>> GetAllJobsAsync();
		Task<JobDto> GetJobByIdAsync(int jobId);
		Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto);
		Task<IEnumerable<JobDto>> GetAppliedJobsAsync(string jobSeekerId);
		Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
		Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);
	}
}
