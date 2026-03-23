using JobConnect.Application.DTOs.SeekerDto;
using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions.Persistence;

public interface IJobSeekerRepository : IRepository<JobSeeker>
{
    Task<JobSeeker?> GetJobSeekerByIdAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<JobSeeker?> GetSeekerProfileAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    void UpdateJobSeeker(JobSeeker jobSeeker);
    Task DeleteJobSeekerAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetSavedJobsAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task SaveJobAsync(string jobSeekerId, int jobId, CancellationToken cancellationToken = default);
    Task UnsaveJobAsync(string jobSeekerId, int jobId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default);
    Task<Job?> GetJobByIdAsync(int jobId, CancellationToken cancellationToken = default);
    Task ApplyForJobAsync(string jobSeekerId, int jobId, string coverLetter, string resumePath, CancellationToken cancellationToken = default);
    Task<IEnumerable<Job>> GetAppliedJobsAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Job> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Employer>> GetAllEmployersAsync(CancellationToken cancellationToken = default);
    Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto, CancellationToken cancellationToken = default);
    Task<ProfileCompletionDto?> GetProfileCompletionAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<Employer?> GetEmployerByIdAsync(string employerId, CancellationToken cancellationToken = default);
    Task UploadResumeAsync(string jobSeekerId, UploadResumeDto uploadDto, CancellationToken cancellationToken = default);
    Task DeleteResumeAsync(string jobSeekerId, int resumeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResumeInfoDto>> GetResumesAsync(string jobSeekerId, CancellationToken cancellationToken = default);
    Task<List<JobSeeker>> GetJobSeekersForMatchingAsync(CancellationToken cancellationToken = default);
}
