using JobConnect.Application.DTOs.EmployerDto;
using JobConnect.Application.DTOs.SeekerDto;
using JobConnect.Domain.Entities;
using SeekerJobDto = JobConnect.Application.DTOs.SeekerDto.JobDto;

namespace JobConnect.Application.Abstractions;

public interface IJobSeekerService
{
    Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId);
    Task<JobSeeker> GetSeekerProfileAsync(string jobSeekerId);
    Task<IEnumerable<SavedJobSummaryDto>> GetSavedJobsAsync(string jobSeekerId);
    Task SaveJobAsync(string jobSeekerId, int jobId);
    Task UnsaveJobAsync(string jobSeekerId, int jobId);
    Task<IEnumerable<SeekerJobDto>> GetAllJobsAsync();
    Task<SeekerJobDto> GetJobByIdAsync(int jobId);
    Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto);
    Task<IEnumerable<AppliedJobSummaryDto>> GetAppliedJobsAsync(string jobSeekerId);
    Task<IEnumerable<EmployerDto>> GetAllEmployersAsync();
    Task<(IEnumerable<SeekerJobDto> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize);
    Task UpdateJobSeekerAsync(JobSeeker jobSeeker);
    Task DeleteJobSeekerAsync(string jobSeekerId);
    Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto);
    Task<ProfileCompletionDto> GetProfileCompletionAsync(string jobSeekerId);
    Task<EmployerProfileDto> GetEmployerByIdAsync(string employerId);
    Task UploadResumeAsync(string jobSeekerId, UploadResumeDto uploadDto);
    Task DeleteResumeAsync(string jobSeekerId, int resumeId);
    Task<IEnumerable<ResumeInfoDto>> GetResumesAsync(string jobSeekerId);
}
