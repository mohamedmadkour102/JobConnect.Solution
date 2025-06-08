//using JobConnect.Apis.DTO_s.SeekerDto;
//using JobConnect.Core.Models;

//namespace JobConnect.Apis.IService
//{
//	public interface IJobSeekerService
//	{
//		Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
//		Task<IEnumerable<SavedJobSummaryDto>> GetSavedJobsAsync(string jobSeekerId);
//		Task SaveJobAsync(string jobSeekerId, int jobId);
//		Task UnsaveJobAsync(string jobSeekerId, int jobId);
//		Task<IEnumerable<JobDto>> GetAllJobsAsync();
//		Task<JobDto> GetJobByIdAsync(int jobId);
//		Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto);
//		Task<IEnumerable<AppliedJobSummaryDto>> GetAppliedJobsAsync(string jobSeekerId);
//		Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
//		Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);
//	}
//}
using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Core.Models;
using JobDto = JobConnect.Apis.DTO_s.SeekerDto.JobDto;

namespace JobConnect.Apis.IService
{
    public interface IJobSeekerService
    {
        Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
        Task<JobSeeker> GetSeekerProfileAsync(string jobSeekerId);
        Task<IEnumerable<SavedJobSummaryDto>> GetSavedJobsAsync(string jobSeekerId);
        Task SaveJobAsync(string jobSeekerId, int jobId);
        Task UnsaveJobAsync(string jobSeekerId, int jobId);
        Task<IEnumerable<JobDto>> GetAllJobsAsync();
        Task<JobDto> GetJobByIdAsync(int jobId);
        Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto);
        Task<IEnumerable<AppliedJobSummaryDto>> GetAppliedJobsAsync(string jobSeekerId);
        Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
        Task<(IEnumerable<DTO_s.SeekerDto.JobDto> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);
        Task UpdateJobSeekerAsync(JobSeeker jobSeeker);
        Task DeleteJobSeekerAsync(string jobSeekerId);
        Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto);
        Task<ProfileCompletionDto> GetProfileCompletionAsync(string jobSeekerId);
        Task<EmployerProfileDto> GetEmployerByIdAsync(string employerId);
        Task UploadResumeAsync(string jobSeekerId, UploadResumeDto uploadDto);
        Task DeleteResumeAsync(string jobSeekerId, int resumeId);
        Task<IEnumerable<ResumeInfoDto>> GetResumesAsync(string jobSeekerId);
    }
}