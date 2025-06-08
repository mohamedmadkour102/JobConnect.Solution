//using JobConnect.Apis.Models;
//using JobConnect.Core.Models;

//namespace JobConnect.Apis.IRepository
//{
//	public interface IJobSeekerRepository
//	{
//		Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
//		Task UpdateJobSeekerAsync(JobSeeker jobSeeker);
//		Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId);
//		Task SaveJobAsync(string jobSeekerId, int jobId);
//		Task UnsaveJobAsync(string jobSeekerId, int jobId);
//		Task<IEnumerable<Job>> GetAllJobsAsync();
//		Task<Job> GetJobByIdAsync(int jobId);
//		Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath);
//		Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId);
//		Task<IEnumerable<Employer>> GetAllEmployersAsync(); // New method
//		Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);


//	}
//}
using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Apis.Models;
using JobConnect.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobConnect.Apis.IRepository
{
    public interface IJobSeekerRepository
    {
        Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
        Task<JobSeeker> GetSeekerProfileAsync(string jobSeekerId);
        Task UpdateJobSeekerAsync(JobSeeker jobSeeker);
        Task DeleteJobSeekerAsync(string jobSeekerId);
        Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId);
        Task SaveJobAsync(string jobSeekerId, int jobId);
        Task UnsaveJobAsync(string jobSeekerId, int jobId);
        Task<IEnumerable<Job>> GetAllJobsAsync();
        Task<Job> GetJobByIdAsync(int jobId);
        Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath);
        Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId);
        Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Employer>> GetAllEmployersAsync();
        Task SaveChangesAsync();
        Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto);
        Task<ProfileCompletionDto> GetProfileCompletionAsync(string jobSeekerId);
    }
}



