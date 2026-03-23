using JobConnect.Application.DTOs.SeekerDto;
using JobConnect.Domain.Entities;
using JobConnect.Application.DTOs.EmployerDto;
using JobConnect.Application.Abstractions.Persistence;
using JobDto = JobConnect.Application.DTOs.SeekerDto.JobDto;
using System.Text.Json;
using JobConnect.Application.Abstractions;

namespace JobConnect.Application.Services
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWebHostEnvironment _environment;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly INotificationService _notificationService;

        public JobSeekerService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment environment, 
            ICloudinaryService cloudinaryService,
            INotificationService notificationService)
        {
            _uow = unitOfWork;
            _environment = environment;
            _cloudinaryService = cloudinaryService;
            _notificationService = notificationService;
        }

        public async Task<JobSeeker> GetJobSeekerByIdAsync(string jobSeekerId)
        {
            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");
            return jobSeeker;
        }

        public async Task<JobSeeker> GetSeekerProfileAsync(string jobSeekerId)
        {
            var jobSeeker = await _uow.JobSeekers.GetSeekerProfileAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker profile not found.");
            return jobSeeker;
        }


        public async Task<IEnumerable<SavedJobSummaryDto>> GetSavedJobsAsync(string jobSeekerId)
        {
            var jobs = await _uow.JobSeekers.GetSavedJobsAsync(jobSeekerId);

            return jobs.Select(job => new SavedJobSummaryDto
            {
                Id = job.Id,
                Title = job.Title ?? string.Empty,
                Location = job.Location ?? string.Empty,
                CreatedAt = GetTimeAgo(job.PostedDate),
                Applicants = job.Applications?.Select(a => new ApplicantDto
                {
                    Id = a.JobSeeker?.Id ?? string.Empty,
                    Name = $"{a.JobSeeker?.FirstName ?? string.Empty} {a.JobSeeker?.LastName ?? string.Empty}".Trim(),
                    Email = a.JobSeeker?.Email ?? string.Empty,
                    CurrentOrDesiredJob = a.JobSeeker?.CurrentOrDesiredJob ?? string.Empty,
                    YearsOfExperience = a.JobSeeker?.YearsOfExperience,
                    ResumeBase64 = a.Resume ?? string.Empty,
                    CoverLetter = a.CoverLetter ?? string.Empty,
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status ?? string.Empty,
                    IsShortlisted = a.IsShortlisted
                })?.ToList() ?? new List<ApplicantDto>(),
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                JobType = job.JobType ?? string.Empty,
                WorkPlace = job.WorkPlace ?? string.Empty,
                PostedDate = GetTimeAgo(job.PostedDate),
                Experience = job.Experience ?? string.Empty,
                SalaryType = job.SalaryType ?? string.Empty,
                Employer = new EmployerInfo
                {
                    Id = job.Employer.Id,
                    Name = $"{job.Employer.FirstName} {job.Employer.LastName}",
                    Email = job.Employer.Email,
                    CompanyName = job.Employer.CompanyName,
                    CompanySize = job.Employer.CompanySize,
                    FoundingDate = job.Employer.FoundingDate,
                    Industry = job.Employer.Industry,
                    LogoBase64 = string.IsNullOrEmpty(job.Employer.LogoUrl) ? null : job.Employer.LogoUrl
                }

            }).ToList();
        }

        public async Task SaveJobAsync(string jobSeekerId, int jobId)
        {
            await _uow.JobSeekers.SaveJobAsync(jobSeekerId, jobId);
            await _uow.SaveChangesAsync();
        }

        public async Task UnsaveJobAsync(string jobSeekerId, int jobId)
        {
            await _uow.JobSeekers.UnsaveJobAsync(jobSeekerId, jobId);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
        {
            var jobs = await _uow.JobSeekers.GetAllJobsAsync();

            return await Task.WhenAll(jobs.Select(async job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Description = job.Description,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Experience = job.Experience,
                Vacancies = job.Vacancies,
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Employer = new EmployerInfo
                {
                    Id = job.Employer.Id,
                    Name = $"{job.Employer.FirstName} {job.Employer.LastName}",
                    Email = job.Employer.Email,
                    CompanyName = job.Employer.CompanyName,
                    CompanySize = job.Employer.CompanySize,
                    FoundingDate = job.Employer.FoundingDate,
                    Industry = job.Employer.Industry,
                    LogoBase64 = string.IsNullOrEmpty(job.Employer.LogoUrl) ? null : job.Employer.LogoUrl
                }
            }));
        }

        public async Task<JobDto> GetJobByIdAsync(int jobId)
        {
            var job = await _uow.JobSeekers.GetJobByIdAsync(jobId);
            if (job == null)
                throw new Exception("Job not found.");

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Description = job.Description,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Experience = job.Experience,
                Vacancies = job.Vacancies,
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Employer = new EmployerInfo
                {
                    Id = job.Employer.Id,
                    Name = $"{job.Employer.FirstName} {job.Employer.LastName}",
                    Email = job.Employer.Email,
                    CompanyName = job.Employer.CompanyName,
                    CompanySize = job.Employer.CompanySize,
                    FoundingDate = job.Employer.FoundingDate,
                    Industry = job.Employer.Industry,
                    LogoBase64 = string.IsNullOrEmpty(job.Employer.LogoUrl) ? null : job.Employer.LogoUrl
                }
            };
        }

        public async Task ApplyForJobAsync(string jobSeekerId, ApplyForJobDto applyDto)
        {
            string resumePath;

            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            var job = await _uow.JobSeekers.GetJobByIdAsync(applyDto.JobId);
            if (job == null)
                throw new Exception("Job not found.");

            if (!string.IsNullOrEmpty(applyDto.SelectedResumePath))
            {
                var selectedResume = jobSeeker.Resumes.FirstOrDefault(r => r.ResumePath == applyDto.SelectedResumePath);
                if (selectedResume == null)
                    throw new Exception("Selected resume not found in your profile.");

                resumePath = selectedResume.ResumePath;
            }
            else if (applyDto.Resume != null)
            {
                resumePath = await _cloudinaryService.UploadAsync(applyDto.Resume);

                var newResume = new JobSeekerResume
                {
                    JobSeekerId = jobSeekerId,
                    ResumePath = resumePath
                };

                jobSeeker.Resumes.Add(newResume);
            }
            else
            {
                throw new Exception("No resume provided.");
            }

            
            await _uow.JobSeekers.ApplyForJobAsync(jobSeekerId, applyDto.JobId, applyDto.CoverLetter, resumePath);
            await _uow.SaveChangesAsync();

           
            await _notificationService.SendNotificationToUserAsync(jobSeekerId, new Notification
            {
                Title = "?? ????? ???? ?????",
                Message = $"?? ????? ???? ??????? {job.Title} ?????",
                Type = NotificationType.ApplicationStatus,
                DataJson = JsonSerializer.Serialize(new { jobId = job.Id }),
                RedirectUrl = $"/jobs/{job.Id}"
            });
        }



        public async Task<IEnumerable<AppliedJobSummaryDto>> GetAppliedJobsAsync(string jobSeekerId)
        {
            var jobs = await _uow.JobSeekers.GetAppliedJobsAsync(jobSeekerId);

            return await Task.WhenAll(jobs.Select(async job => new AppliedJobSummaryDto
            {
                Id = job.Id,
                Title = job.Title ?? string.Empty,
                SalaryType = job.SalaryType,
                Location = job.Location ?? string.Empty,
                CreatedAt = GetTimeAgo(job.PostedDate),
                Applicants = job.Applications?.Select(a => new ApplicantDto
                {
                    Id = a.JobSeeker?.Id ?? string.Empty,
                    Name = $"{a.JobSeeker?.FirstName ?? string.Empty} {a.JobSeeker?.LastName ?? string.Empty}".Trim(),
                    Email = a.JobSeeker?.Email ?? string.Empty,
                    CurrentOrDesiredJob = a.JobSeeker?.CurrentOrDesiredJob ?? string.Empty,
                    YearsOfExperience = a.JobSeeker?.YearsOfExperience,
                    ResumeBase64 = a.Resume ?? string.Empty,
                    CoverLetter = a.CoverLetter ?? string.Empty,
                    ApplicationDate = a.ApplicationDate,
                    Status = a.Status,
                    IsShortlisted = a.IsShortlisted
                })?.ToList() ?? new List<ApplicantDto>(),
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                JobType = job.JobType ?? string.Empty,
                WorkPlace = job.WorkPlace ?? string.Empty,
                Status = job.Status ?? string.Empty,
                PostedDate = GetTimeAgo(job.PostedDate),
                Experience = job.Experience ?? string.Empty,
                Employer = new EmployerInfo
                {
                    Id = job.Employer.Id,
                    Name = $"{job.Employer.FirstName} {job.Employer.LastName}",
                    Email = job.Employer.Email,
                    CompanyName = job.Employer.CompanyName,
                    CompanySize = job.Employer.CompanySize,
                    FoundingDate = job.Employer.FoundingDate,
                    Industry = job.Employer.Industry,
                    LogoBase64 = string.IsNullOrEmpty(job.Employer.LogoUrl) ? null : job.Employer.LogoUrl
                }
            }));
        }

        public async Task<IEnumerable<EmployerDto>> GetAllEmployersAsync()
        {
            var employers = await _uow.JobSeekers.GetAllEmployersAsync();

            return employers.Select(employer => new EmployerDto
            {
                Id = employer.Id,
                Name = $"{employer.FirstName} {employer.LastName}",
                Email = employer.Email,
                CompanyName = employer.CompanyName,
                Industry = employer.Industry,
                Address = employer.Address,
                JobsPostedCount = employer.Jobs?.Count ?? 0
            }).ToList();
        }

        public async Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetAllJobsPaginatedAsync(int pageNumber, int pageSize)
        {
            var (jobs, totalCount) = await _uow.JobSeekers.GetAllJobsPaginatedAsync(pageNumber, pageSize);

            var jobDtos = await Task.WhenAll(jobs.Select(async job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
               
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Description = job.Description,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Experience = job.Experience,
                Vacancies = job.Vacancies,
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Employer = new EmployerInfo
                {
                    Id = job.Employer.Id,
                    Name = $"{job.Employer.FirstName} {job.Employer.LastName}",
                    Email = job.Employer.Email,
                    CompanyName = job.Employer.CompanyName,
                    CompanySize = job.Employer.CompanySize,
                    FoundingDate = job.Employer.FoundingDate,
                    Industry = job.Employer.Industry,
                    LogoBase64 = string.IsNullOrEmpty(job.Employer.LogoUrl) ? null : job.Employer.LogoUrl
                }
            }));

            return (jobDtos, totalCount);
        }
        //public async Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto)
        //{
        //    var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
        //    if (jobSeeker == null)
        //        throw new Exception("JobSeeker not found.");

        //    var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == applyDto.ResumeId);
        //    if (resume == null)
        //        throw new Exception("Selected resume not found in your profile.");

        //    await _uow.JobSeekers.ApplyForJobByResumeIdAsync(jobSeekerId, applyDto);

        //}
        public async Task ApplyForJobByResumeIdAsync(string jobSeekerId, ApplyForJobByResumeIdDto applyDto)
        {
            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == applyDto.ResumeId);
            if (resume == null)
                throw new Exception("Selected resume not found in your profile.");

            var job = await _uow.JobSeekers.GetJobByIdAsync(applyDto.JobId);
            if (job == null)
                throw new Exception("Job not found.");

            await _uow.JobSeekers.ApplyForJobByResumeIdAsync(jobSeekerId, applyDto);

            await _uow.SaveChangesAsync();

            await _notificationService.SendNotificationToUserAsync(jobSeekerId, new Notification
            {
                Title = "?? ????? ???? ?????",
                Message = $"?? ????? ???? ??????? {job.Title} ?????",
                Type = NotificationType.ApplicationStatus,
                DataJson = JsonSerializer.Serialize(new { jobId = job.Id }),
                RedirectUrl = $"/jobs/{job.Id}"
            });
        }
 



        public async Task<EmployerProfileDto> GetEmployerByIdAsync(string employerId)
        {
            var employer = await _uow.JobSeekers.GetEmployerByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");

            return new EmployerProfileDto
            {
                Id = employer.Id,
                Name = $"{employer.FirstName} {employer.LastName}",
                Email = employer.Email,
                CompanyName = employer.CompanyName,
                CompanySize = employer.CompanySize,
                Website = employer.Website,
                Industry = employer.Industry,
                Address = employer.Address,
                CompanyDescription = employer.CompanyDescription,
                LogoUrl = employer.LogoUrl,
                FoundingDate = employer.FoundingDate,
                PhoneNumber = employer.PhoneNumber,
                Jobs = employer.Jobs.Select(j => new JobEmpDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Status = j.Status,
                    JobType = j.JobType,
                    WorkPlace = j.WorkPlace,
                    PostedDate = GetTimeAgo(j.PostedDate),
                    Location = j.Location,
                    Description = j.Description,
                    MinSalary = j.MinSalary,
                    MaxSalary = j.MaxSalary,
                    SalaryType = j.SalaryType,
                    Education = j.Education,
                    Experience = j.Experience,
                    Vacancies = j.Vacancies
                }).ToList()
            };
        }

        public async Task UpdateJobSeekerAsync(JobSeeker jobSeeker)
        {
            _uow.JobSeekers.UpdateJobSeeker(jobSeeker);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteJobSeekerAsync(string jobSeekerId)
        {
            await _uow.JobSeekers.DeleteJobSeekerAsync(jobSeekerId);
            await _uow.SaveChangesAsync();
        }
        public async Task<ProfileCompletionDto> GetProfileCompletionAsync(string jobSeekerId)
        {
            var completion = await _uow.JobSeekers.GetProfileCompletionAsync(jobSeekerId);
            if (completion == null)
                throw new Exception("JobSeeker not found.");
            return completion;
        }
        public async Task UploadResumeAsync(string jobSeekerId, UploadResumeDto uploadDto)
        {
            if (uploadDto?.Resume == null)
                throw new Exception("Resume file is required.");

            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            var resumePath = await _cloudinaryService.UploadAsync(uploadDto.Resume);

            var newResume = new JobSeekerResume
            {
                JobSeekerId = jobSeekerId,
                ResumePath = resumePath,
                ResumeName = uploadDto.Resume.FileName,
                UploadDate = DateTime.UtcNow
            };

            jobSeeker.Resumes.Add(newResume);
            _uow.JobSeekers.UpdateJobSeeker(jobSeeker);
            await _uow.SaveChangesAsync();
        }

        public async Task DeleteResumeAsync(string jobSeekerId, int resumeId)
        {
            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            var resume = jobSeeker.Resumes.FirstOrDefault(r => r.Id == resumeId);
            if (resume == null)
                throw new Exception("Resume not found.");

            jobSeeker.Resumes.Remove(resume);
            _uow.JobSeekers.UpdateJobSeeker(jobSeeker);
            await _uow.SaveChangesAsync();
        }

        public async Task<IEnumerable<ResumeInfoDto>> GetResumesAsync(string jobSeekerId)
        {
            var jobSeeker = await _uow.JobSeekers.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                throw new Exception("JobSeeker not found.");

            return jobSeeker.Resumes.Select(r => new ResumeInfoDto
            {
                Id = r.Id,
                ResumeName = r.ResumeName,
                ResumePath = r.ResumePath
                
            }).ToList();
        }

        private string GetTimeAgo(DateTime date)
        {
            TimeSpan timeSpan = DateTime.UtcNow - date;
            if (timeSpan.TotalDays >= 7)
                return $"{(int)timeSpan.TotalDays / 7} week ago";
            else if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            return "Today";
        }

        private int CalculateDaysRemaining(DateTime expirationDate)
        {
            int daysRemaining = (expirationDate - DateTime.UtcNow).Days;
            return daysRemaining > 0 ? daysRemaining : 0;
        }
    }
}