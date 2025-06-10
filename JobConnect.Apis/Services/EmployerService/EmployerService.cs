using Microsoft.AspNetCore.Identity;
using JobConnect.Core.Models;
using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.IRepository;
using JobConnect.Apis.IService;
using JobConnect.Apis.Models;
using JobConnect.Apis.DTO_s.SeekerDto;
using JobDto = JobConnect.Apis.DTO_s.EmployerDto.JobDto;
using Microsoft.EntityFrameworkCore;
using JobConnect.Repository.Data;


namespace JobConnect.Apis.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly AppDbContext _context;

        public EmployerService(IEmployerRepository employerRepository, UserManager<User> userManager, IWebHostEnvironment environment,
            ICloudinaryService cloudinaryService ,AppDbContext context )
        {
            _employerRepository = employerRepository;
            _userManager = userManager;
            _environment = environment;
            _cloudinaryService = cloudinaryService;
            _context = context;
        }

        public async Task<Employer> GetEmployerByIdAsync(string employerId)
        {
            var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");
            return employer;
        }

        public async Task UpdateCompanyInfoAsync(string employerId, UpdateCompanyInfoDto dto)
        {
            var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");

            employer.CompanyName = dto.CompanyName ?? employer.CompanyName;
            employer.CompanyDescription = dto.CompanyDescription ?? employer.CompanyDescription;

            if (dto.Logo != null)
            {
                employer.LogoUrl = await _cloudinaryService.UploadAsync(dto.Logo);
            }

            await _employerRepository.UpdateEmployerAsync(employer);
        }

        public async Task UpdateFoundingInfoAsync(string employerId, UpdateFoundingInfoDto dto)
        {
            var employer = await _employerRepository.GetEmployerByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");

            employer.Industry = dto.Industry ?? employer.Industry;
            employer.CompanySize = dto.CompanySize ?? employer.CompanySize;
            employer.FoundingDate = dto.FoundingDate ?? employer.FoundingDate;
            employer.Website = dto.Website ?? employer.Website;
            employer.PhoneNumber = dto.PhoneNumber ?? employer.PhoneNumber;

            await _employerRepository.UpdateEmployerAsync(employer);
        }

        public async Task ChangePasswordAsync(string employerId, ChangePasswordDto dto)
        {
            var employer = await _userManager.FindByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");

            var passwordCheck = await _userManager.CheckPasswordAsync(employer, dto.CurrentPassword);
            if (!passwordCheck)
                throw new Exception("Current password is incorrect.");

            var result = await _userManager.ChangePasswordAsync(employer, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                throw new Exception("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task<IEnumerable<JobDto>> GetRecentJobsAsync(string employerId)
        {
            var jobs = await _employerRepository.GetRecentJobsByEmployerAsync(employerId);

            return await Task.WhenAll(jobs.Select(async job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                MaxSalary = job.MaxSalary,
                MinSalary = job.MinSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Vacancies = job.Vacancies,
                ExpirationDate = job.ExpirationDate,
                Description = job.Description,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Applicants = (await Task.WhenAll(job.Applications.Select(async a => new ApplicantDto
                {
                    Id = a.JobSeekerId,
                    Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                    Email = a.JobSeeker.Email,
                    CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                    YearsOfExperience = a.JobSeeker.YearsOfExperience,
                    ResumeBase64 = string.IsNullOrEmpty(a.Resume) ? null : a.Resume,
                    CoverLetter = a.CoverLetter,
                    ApplicationDate = a.ApplicationDate,
                    IsShortlisted = a.IsShortlisted
                }))).ToList()
            }));
        }

        public async Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(string employerId)
        {
            var jobs = await _employerRepository.GetJobsByEmployerAsync(employerId);

            return await Task.WhenAll(jobs.Select(async job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                MaxSalary = job.MaxSalary,
                MinSalary = job.MinSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Vacancies = job.Vacancies,
                ExpirationDate = job.ExpirationDate,
                Description = job.Description,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Applicants = (await Task.WhenAll(job.Applications.Select(async a => new ApplicantDto
                {
                    Id = a.JobSeekerId,
                    Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                    Email = a.JobSeeker.Email,
                    CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                    YearsOfExperience = a.JobSeeker.YearsOfExperience,
                    ResumeBase64 = string.IsNullOrEmpty(a.Resume) ? null : a.Resume,
                    CoverLetter = a.CoverLetter,
                    ApplicationDate = a.ApplicationDate,
                    IsShortlisted = a.IsShortlisted
                }))).ToList()
            }));
        }

        public async Task<JobDto?> GetJobByIdAsync(int jobId, string employerId)
        {
            var job = await _employerRepository.GetJobByIdAsync(jobId, employerId);
            if (job == null) return null;

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                MaxSalary = job.MaxSalary,
                MinSalary = job.MinSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Vacancies = job.Vacancies,
                ExpirationDate = job.ExpirationDate,
                Description = job.Description,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Applicants = (await Task.WhenAll(job.Applications.Select(async a => new ApplicantDto
                {
                    Id = a.JobSeekerId,
                    Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                    Email = a.JobSeeker.Email,
                    CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                    YearsOfExperience = a.JobSeeker.YearsOfExperience,
                    ResumeBase64 = string.IsNullOrEmpty(a.Resume) ? null : a.Resume,
                    CoverLetter = a.CoverLetter,
                    ApplicationDate = a.ApplicationDate,
                    IsShortlisted = a.IsShortlisted
                }))).ToList()
            };
        }

        public async Task AddJobAsync(string employerId, CreateJobDto jobDto)
        {
            var job = new Job
            {
                Title = jobDto.Title,
                Description = jobDto.Description,
                MinSalary = jobDto.MinSalary,
                MaxSalary = jobDto.MaxSalary,
                SalaryType = jobDto.SalaryType,
                Education = jobDto.Education,
                Experience = jobDto.Experience,
                Vacancies = jobDto.Vacancies,
                ExpirationDate = jobDto.ExpirationDate,
                JobType = jobDto.JobType,
                WorkPlace = jobDto.WorkPlace,
                Status = jobDto.Status,
                Location = jobDto.Location,
                EmployerId = employerId
            };

            if (jobDto.Tags != null && jobDto.Tags.Any())
            {
                job.Tags = jobDto.Tags.Select(tag => new JobTag { Tag = tag }).ToList();
            }

            if (jobDto.Responsibilities != null && jobDto.Responsibilities.Any())
            {
                job.Responsibilities = jobDto.Responsibilities.Select(resp => new JobResponsibility { Responsibility = resp }).ToList();
            }

            await _employerRepository.AddJobAsync(job);
        }

        public async Task UpdateJobAsync(int jobId, string employerId, UpdateJobDto jobDto)
        {
            var job = await _employerRepository.GetJobByIdAsync(jobId, employerId);
            if (job == null) return;

            job.Title = jobDto.Title;
            job.Description = jobDto.Description;
            job.MaxSalary = jobDto.MaxSalary;
            job.MinSalary = jobDto.MinSalary;
            job.SalaryType = jobDto.SalaryType;
            job.Education = jobDto.Education;
            job.Experience = jobDto.Experience;
            job.Vacancies = jobDto.Vacancies;
            job.ExpirationDate = jobDto.ExpirationDate;
            job.JobType = jobDto.JobType;
            job.WorkPlace = jobDto.WorkPlace;
            job.Status = jobDto.Status;
            job.Location = jobDto.Location;

            job.Tags.Clear();
            if (jobDto.Tags != null && jobDto.Tags.Any())
            {
                job.Tags = jobDto.Tags.Select(tag => new JobTag { Tag = tag }).ToList();
            }

            job.Responsibilities.Clear();
            if (jobDto.Responsibilities != null && jobDto.Responsibilities.Any())
            {
                job.Responsibilities = jobDto.Responsibilities.Select(resp => new JobResponsibility { Responsibility = resp }).ToList();
            }

            await _employerRepository.UpdateJobAsync(job);
        }

        public async Task DeleteJobAsync(int jobId, string employerId)
        {
            await _employerRepository.DeleteJobAsync(jobId, employerId);
        }

        public async Task<JobStatsDto> GetJobStatsAsync(string employerId)
        {
            return new JobStatsDto
            {
                JobsCount = await _employerRepository.GetJobsCountAsync(employerId),
                CandidatesCount = await _employerRepository.GetCandidatesCountAsync(employerId)
            };
        }

        public async Task AddToShortlistAsync(int jobId, string jobSeekerId)
        {
            await _employerRepository.AddToShortlistAsync(jobId, jobSeekerId);
        }

        public async Task RemoveFromShortlistAsync(int jobId, string jobSeekerId)
        {
            await _employerRepository.RemoveFromShortlistAsync(jobId, jobSeekerId);
        }

        public async Task<IEnumerable<ShortlistedJobSeekerDto>> GetShortlistedJobSeekersAsync(int jobId, string employerId)
        {
            var applications = await _employerRepository.GetShortlistedJobSeekersAsync(jobId, employerId);

            return applications.Select(a => new ShortlistedJobSeekerDto
            {
                Id = a.JobSeekerId,
                Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                Email = a.JobSeeker.Email,
                CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                YearsOfExperience = a.JobSeeker.YearsOfExperience,
                Resume = a.Resume,
                CoverLetter = a.CoverLetter,
                ApplicationDate = a.ApplicationDate
            }).ToList();
        }

        public async Task<(IEnumerable<JobDto> Jobs, int TotalCount)> GetJobsByEmployerPaginatedAsync(string employerId, int pageNumber, int pageSize)
        {
            var (jobs, totalCount) = await _employerRepository.GetJobsByEmployerPaginatedAsync(employerId, pageNumber, pageSize);

            var jobDtos = await Task.WhenAll(jobs.Select(async job => new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Status = job.Status,
                ApplicationsCount = job.Applications.Count,
                JobType = job.JobType,
                WorkPlace = job.WorkPlace,
                MaxSalary = job.MaxSalary,
                MinSalary = job.MinSalary,
                SalaryType = job.SalaryType,
                Education = job.Education,
                Vacancies = job.Vacancies,
                ExpirationDate = job.ExpirationDate,
                Description = job.Description,
                DaysRemaining = CalculateDaysRemaining(job.ExpirationDate),
                PostedDate = GetTimeAgo(job.PostedDate),
                Location = job.Location,
                Tags = job.Tags.Select(t => t.Tag).ToList(),
                Responsibilities = job.Responsibilities.Select(r => r.Responsibility).ToList(),
                Applicants = (await Task.WhenAll(job.Applications.Select(async a => new ApplicantDto
                {
                    Id = a.JobSeekerId,
                    Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                    Email = a.JobSeeker.Email,
                    CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                    YearsOfExperience = a.JobSeeker.YearsOfExperience,
                    ResumeBase64 = string.IsNullOrEmpty(a.Resume) ? null : a.Resume,
                    CoverLetter = a.CoverLetter,
                    ApplicationDate = a.ApplicationDate,
                    IsShortlisted = a.IsShortlisted
                }))).ToList()
            }));

            return (jobDtos, totalCount);
        }

        public async Task DeleteEmployerAccountAsync(string employerId)
        {
            var employer = await _userManager.FindByIdAsync(employerId);
            if (employer == null)
                throw new Exception("Employer not found.");

            var jobs = await _employerRepository.GetJobsByEmployerAsync(employerId);
            foreach (var job in jobs)
            {
                await _employerRepository.DeleteJobAsync(job.Id, employerId);
            }

            var result = await _userManager.DeleteAsync(employer);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to delete employer: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<SeekerProfileDto?> GetJobSeekerByIdAsync(string employerId, string jobSeekerId)
        {
            var jobs = await _employerRepository.GetJobsByEmployerAsync(employerId);
            var applicationExists = jobs.SelectMany(j => j.Applications).Any(a => a.JobSeekerId == jobSeekerId);

            if (!applicationExists)
                return null;

            var jobSeeker = await _employerRepository.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                return null;

            return new SeekerProfileDto
            {
                Id = jobSeeker.Id,
                FirstName = jobSeeker.FirstName,
                LastName = jobSeeker.LastName,
                Email = jobSeeker.Email,
                Address = jobSeeker.Address,
                YearsOfExperience = jobSeeker.YearsOfExperience,
                Degree = jobSeeker.Degree,
                CurrentOrDesiredJob = jobSeeker.CurrentOrDesiredJob,
                Bio = jobSeeker.Bio,
                CoverLetter = jobSeeker.CoverLetter,
                DateOfBirth = jobSeeker.DateOfBirth,
                Nationality = jobSeeker.Nationality,
                MaritalStatus = jobSeeker.MaritalStatus,
                Gender = jobSeeker.Gender,
                Education = jobSeeker.Education,
                Portfolio = jobSeeker.Portfolio,
                FacebookLink = jobSeeker.FacebookLink,
                TwitterLink = jobSeeker.TwitterLink,
                InstagramLink = jobSeeker.InstagramLink,
                LinkedInLink = jobSeeker.LinkedInLink,
                CollegeName = jobSeeker.CollegeName,
                University = jobSeeker.University,
                Resumes = jobSeeker.Resumes.Select(r => new ResumeDto
                {
                    ResumePath = r.ResumePath,
                    ResumeName = r.ResumeName,
                    UploadDate = r.UploadDate
                }).ToList(),
                Certifications = jobSeeker.Certifications.Select(c => new CertificationDto
                {
                    CertificationName = c.CertificationName,
                    //IssuingOrganization = c.IssuingOrganization,
                    //IssueDate = c.IssueDate,
                    //ExpiryDate = c.ExpiryDate
                }).ToList(),
                CompanyWorkedAt = jobSeeker.CompanyWorkedAt.Select(c => new CompanyWorkedAtDto
                {
                    CompanyName = c.CompanyName,
                 //   StartDate = c.StartDate,
                   // EndDate = c.EndDate
                }).ToList(),
                Skills = jobSeeker.Skills.Select(s => new SkillDto
                {
                    SkillName = s.SkillName,
                  //  ProficiencyLevel = s.ProficiencyLevel
                }).ToList(),
                WorkedAs = jobSeeker.WorkedAs.Select(w => new WorkedAsDto
                {
                    JobTitle = w.JobTitle,
                   // StartDate = w.StartDate,
                    //EndDate = w.EndDate
                }).ToList()
            };
        }

        public async Task<IEnumerable<ResumeDto>> GetSeekerResumesWithIdAsync(string jobSeekerId)
        {
            var jobSeeker = await _employerRepository.GetJobSeekerByIdAsync(jobSeekerId);
            if (jobSeeker == null)
                return Enumerable.Empty<ResumeDto>();

            return jobSeeker.Resumes.Select(r => new ResumeDto
            {
                ResumePath = r.ResumePath,
                ResumeName = r.ResumeName,
                UploadDate = r.UploadDate
            });
        }
         public async Task<IEnumerable<JobApplicantWithResumeDto>> GetApplicantsWithResumeAsync(int jobId, string employerId)
        {
            // Verify the job exists and belongs to the employer
            var job = await _employerRepository.GetJobByIdAsync(jobId, employerId);
            if (job == null)
                throw new Exception("Job not found or you don't have access to it.");

            // Get applications for the job
            var applications = await _employerRepository.GetApplicationsByJobAsync(jobId);
            if (!applications.Any())
                return Enumerable.Empty<JobApplicantWithResumeDto>();

            // Map applications to DTO
            return await Task.WhenAll(applications.Select(async a => new JobApplicantWithResumeDto
            {
                Id = a.JobSeekerId,
                Name = $"{a.JobSeeker.FirstName} {a.JobSeeker.LastName}",
                Email = a.JobSeeker.Email,
                CurrentOrDesiredJob = a.JobSeeker.CurrentOrDesiredJob,
                YearsOfExperience = a.JobSeeker.YearsOfExperience,
                Resume = string.IsNullOrEmpty(a.Resume) ? null : a.Resume,
                CoverLetter = a.CoverLetter,
                ApplicationDate = a.ApplicationDate,
                IsShortlisted = a.IsShortlisted
            }));
        }
        public async Task<bool> HireApplicantAsync(string employerId, int jobId, string jobSeekerId)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
            if (job == null) return false;

            return await _employerRepository.HireApplicantAsync(jobId, jobSeekerId);
        }

        public async Task<bool> RejectApplicantAsync(string employerId, int jobId, string jobSeekerId)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);
            if (job == null) return false;

            return await _employerRepository.RejectApplicantAsync(jobId, jobSeekerId);
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