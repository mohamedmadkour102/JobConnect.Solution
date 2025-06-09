using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JobConnect.Apis.Helpers;
using JobConnect.Core.Models;
using Microsoft.EntityFrameworkCore;
using JobConnect.Repository.Data;

namespace JobConnect.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobSeekerController : ControllerBase
    {
        private readonly IJobSeekerService _jobSeekerService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;

        public JobSeekerController(IJobSeekerService jobSeekerService, IWebHostEnvironment environment , AppDbContext context)
        {
            _jobSeekerService = jobSeekerService;
            _environment = environment;
            _context = context;
        }

        [HttpGet("GetSavedJobs")]
        public async Task<IActionResult> GetSavedJobs()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobs = await _jobSeekerService.GetSavedJobsAsync(jobSeekerId);
            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No saved jobs found.", data = new List<object>() });

            return Ok(new { message = "Saved jobs retrieved successfully.", data = jobs });
        }

        [HttpPost("SaveJob")]
        public async Task<IActionResult> SaveJob([FromBody] SaveJobRequestDto request)
        {
            if (request.JobId <= 0)
                return BadRequest(new { message = "Invalid job ID." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                await _jobSeekerService.SaveJobAsync(jobSeekerId, request.JobId);
                return Ok(new { message = "Job saved successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("UnsaveJob")]
        public async Task<IActionResult> UnsaveJob([FromBody] SaveJobRequestDto request)
        {
            if (request.JobId <= 0)
                return BadRequest(new { message = "Invalid job ID." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _jobSeekerService.UnsaveJobAsync(jobSeekerId, request.JobId);
            return Ok(new { message = "Job unsaved successfully." });
        }

        [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobSeekerService.GetAllJobsAsync();
            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No jobs available.", data = new List<object>() });

            return Ok(new { message = "Jobs retrieved successfully.", data = jobs });
        }

        [HttpGet("GetJobById/{jobId}")]
        public async Task<IActionResult> GetJobById(int jobId)
        {
            var job = await _jobSeekerService.GetJobByIdAsync(jobId);
            if (job == null)
                return NotFound(new { message = $"Job with ID {jobId} not found." });

            return Ok(new { message = "Job details retrieved successfully.", data = job });
        }

        [HttpPost("ApplyForJob")]
        public async Task<IActionResult> ApplyForJob([FromForm] ApplyForJobDto applyDto)
        {
            if (applyDto == null || applyDto.JobId <= 0)
                return BadRequest(new { message = "Invalid application data." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _jobSeekerService.ApplyForJobAsync(jobSeekerId, applyDto);
            return Ok(new { message = "Application submitted successfully." });
        }

        [HttpGet("GetAppliedJobs")]
        public async Task<IActionResult> GetAppliedJobs()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobs = await _jobSeekerService.GetAppliedJobsAsync(jobSeekerId);
            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No applied jobs found.", data = new List<object>() });

            return Ok(new { message = "Applied jobs retrieved successfully.", data = jobs });
        }

        [HttpGet("GetAllEmployers")]
        public async Task<IActionResult> GetAllEmployers()
        {
            var employers = await _jobSeekerService.GetAllEmployersAsync();
            if (employers == null || !employers.Any())
                return Ok(new { message = "No employers found.", data = new List<object>() });

            return Ok(new { message = "Employers retrieved successfully.", data = employers });
        }

        [HttpGet("GetAllJobsPaginated")]
        public async Task<IActionResult> GetAllJobsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { message = "Page number and page size must be greater than 0." });

            var (jobs, totalCount) = await _jobSeekerService.GetAllJobsPaginatedAsync(pageNumber, pageSize);

            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No jobs available.", data = new List<object>(), totalCount = 0 });

            return Ok(new
            {
                message = "Jobs retrieved successfully with pagination.",
                data = jobs,
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        [HttpGet("GetResume/{resumePath}")]
        public async Task<IActionResult> GetResume(string resumePath)
        {
            if (string.IsNullOrEmpty(resumePath))
                return BadRequest(new { message = "Resume path is required." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobSeeker = await _jobSeekerService.GetJobSeekerByIdAsync(jobSeekerId);

            var resume = jobSeeker.Resumes.FirstOrDefault(r => r.ResumePath == resumePath);
            if (resume == null)
                return NotFound(new { message = "Resume not found." });

            var base64String = await FileHelper.ConvertRelativeFileToBase64Async(_environment.WebRootPath, resumePath);
            if (base64String == null)
                return NotFound(new { message = "Resume file not found on server." });

            return Ok(new { message = "Resume retrieved successfully.", data = base64String });
        }

        [HttpGet("GetSeekerProfile")]
        public async Task<IActionResult> GetSeekerProfile()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobSeeker = await _jobSeekerService.GetSeekerProfileAsync(jobSeekerId);
            if (jobSeeker == null)
                return NotFound(new { message = "JobSeeker not found." });

            var profile = new SeekerProfileDto
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
                    //StartDate = c.StartDate,
                    //EndDate = c.EndDate
                }).ToList(),
                Skills = jobSeeker.Skills.Select(s => new SkillDto
                {
                    SkillName = s.SkillName,
                   // ProficiencyLevel = s.ProficiencyLevel
                }).ToList(),
                WorkedAs = jobSeeker.WorkedAs.Select(w => new WorkedAsDto
                {
                    JobTitle = w.JobTitle,
                    //StartDate = w.StartDate,
                    //EndDate = w.EndDate
                }).ToList()
            };

            return Ok(new { message = "Profile retrieved successfully.", data = profile });
        }

        [HttpPut("UpdateSeekerProfile")]
        public async Task<IActionResult> UpdateSeekerProfile([FromBody] UpdateSeekerProfileDto updateDto)
        {
            if (updateDto == null)
                return BadRequest(new { message = "Invalid profile data." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobSeeker = await _jobSeekerService.GetSeekerProfileAsync(jobSeekerId);
            if (jobSeeker == null)
                return NotFound(new { message = "JobSeeker not found." });

            // Update scalar properties if provided
            if (!string.IsNullOrEmpty(updateDto.Address)) jobSeeker.Address = updateDto.Address;
            if (updateDto.YearsOfExperience.HasValue) jobSeeker.YearsOfExperience = updateDto.YearsOfExperience;
            if (!string.IsNullOrEmpty(updateDto.Degree)) jobSeeker.Degree = updateDto.Degree;
            if (!string.IsNullOrEmpty(updateDto.CurrentOrDesiredJob)) jobSeeker.CurrentOrDesiredJob = updateDto.CurrentOrDesiredJob;
            if (!string.IsNullOrEmpty(updateDto.Bio)) jobSeeker.Bio = updateDto.Bio;
            if (!string.IsNullOrEmpty(updateDto.CoverLetter)) jobSeeker.CoverLetter = updateDto.CoverLetter;
            if (updateDto.DateOfBirth.HasValue) jobSeeker.DateOfBirth = updateDto.DateOfBirth;
            if (!string.IsNullOrEmpty(updateDto.Nationality)) jobSeeker.Nationality = updateDto.Nationality;
            if (!string.IsNullOrEmpty(updateDto.MaritalStatus)) jobSeeker.MaritalStatus = updateDto.MaritalStatus;
            if (!string.IsNullOrEmpty(updateDto.Gender)) jobSeeker.Gender = updateDto.Gender;
            if (!string.IsNullOrEmpty(updateDto.Education)) jobSeeker.Education = updateDto.Education;
            if (!string.IsNullOrEmpty(updateDto.Portfolio)) jobSeeker.Portfolio = updateDto.Portfolio;
            if (!string.IsNullOrEmpty(updateDto.FacebookLink)) jobSeeker.FacebookLink = updateDto.FacebookLink;
            if (!string.IsNullOrEmpty(updateDto.TwitterLink)) jobSeeker.TwitterLink = updateDto.TwitterLink;
            if (!string.IsNullOrEmpty(updateDto.InstagramLink)) jobSeeker.InstagramLink = updateDto.InstagramLink;
            if (!string.IsNullOrEmpty(updateDto.LinkedInLink)) jobSeeker.LinkedInLink = updateDto.LinkedInLink;
            if (!string.IsNullOrEmpty(updateDto.CollegeName)) jobSeeker.CollegeName = updateDto.CollegeName;
            if (!string.IsNullOrEmpty(updateDto.University)) jobSeeker.University = updateDto.University;

            // Append Certifications (instead of full replace)
            if (updateDto.Certifications != null && updateDto.Certifications.Any())
            {
                foreach (var c in updateDto.Certifications)
                {
                    var exists = jobSeeker.Certifications.Any(x =>
                        x.CertificationName == c.CertificationName /*&&  x.IssuingOrganization == c.IssuingOrganization &&x.IssueDate == c.IssueDate*/);

                    if (!exists)
                    {
                        jobSeeker.Certifications.Add(new JobSeekerCertification
                        {
                            CertificationName = c.CertificationName,
                            //IssuingOrganization = c.IssuingOrganization,
                            //IssueDate = c.IssueDate,
                            //ExpiryDate = c.ExpiryDate,
                            JobSeekerId = jobSeekerId
                        });
                    }
                }
            }

            // Append CompanyWorkedAt
            if (updateDto.CompanyWorkedAt != null && updateDto.CompanyWorkedAt.Any())
            {
                foreach (var c in updateDto.CompanyWorkedAt)
                {
                    var exists = jobSeeker.CompanyWorkedAt.Any(x =>
                        x.CompanyName == c.CompanyName/* && x.StartDate == c.StartDate*/);

                    if (!exists)
                    {
                        jobSeeker.CompanyWorkedAt.Add(new JobSeekerCompanyWorkedAt
                        {
                            CompanyName = c.CompanyName,
                           // StartDate = c.StartDate,
                            //EndDate = c.EndDate,
                            JobSeekerId = jobSeekerId
                        });
                    }
                }
            }

            // Append Skills
            if (updateDto.Skills != null && updateDto.Skills.Any())
            {
                foreach (var s in updateDto.Skills)
                {
                    var exists = jobSeeker.Skills.Any(x => x.SkillName == s.SkillName);

                    if (!exists)
                    {
                        jobSeeker.Skills.Add(new JobSeekerSkill
                        {
                            SkillName = s.SkillName,
                           // ProficiencyLevel = s.ProficiencyLevel,
                            JobSeekerId = jobSeekerId
                        });
                    }
                }
            }

            // Append WorkedAs
            if (updateDto.WorkedAs != null && updateDto.WorkedAs.Any())
            {
                foreach (var w in updateDto.WorkedAs)
                {
                    var exists = jobSeeker.WorkedAs.Any(x =>
                        x.JobTitle == w.JobTitle /*&& x.StartDate == w.StartDate*/);

                    if (!exists)
                    {
                        jobSeeker.WorkedAs.Add(new JobSeekerWorkedAs
                        {
                            JobTitle = w.JobTitle,
                            //StartDate = w.StartDate,
                            //EndDate = w.EndDate,
                            JobSeekerId = jobSeekerId
                        });
                    }
                }
            }

            await _jobSeekerService.UpdateJobSeekerAsync(jobSeeker);

            return Ok(new { message = "Profile updated successfully." });
        }


        [HttpDelete("DeleteSeekerProfile")]
        public async Task<IActionResult> DeleteSeekerProfile()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _jobSeekerService.DeleteJobSeekerAsync(jobSeekerId);
            return Ok(new { message = "Profile deleted successfully." });
        }
        [HttpPost("ApplyForJobByResumeId/{jobId}/{resumeId}")]
        public async Task<IActionResult> ApplyForJobByResumeId(int jobId, int resumeId , string CoverLetter )
        {
            if (jobId <= 0 || resumeId <= 0)
                return BadRequest(new { message = "Invalid job ID or resume ID." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var applyDto = new ApplyForJobByResumeIdDto
            {
                JobId = jobId,
                ResumeId = resumeId,
                CoverLetter = CoverLetter
            };

            await _jobSeekerService.ApplyForJobByResumeIdAsync(jobSeekerId, applyDto);
            return Ok(new { message = "Application submitted successfully." });
        }

        [HttpGet("GetProfileCompletion")]
        public async Task<IActionResult> GetProfileCompletion()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var completion = await _jobSeekerService.GetProfileCompletionAsync(jobSeekerId);
            if (completion == null)
                return NotFound(new { message = "Profile completion data not found." });

            return Ok(new { message = "Profile completion retrieved successfully.", data = completion });
        }

        [HttpGet("GetEmployerById/{employerId}")]
        public async Task<IActionResult> GetEmployerById(string employerId)
        {
            if (string.IsNullOrEmpty(employerId))
                return BadRequest(new { message = "Employer ID is required." });

            var employer = await _jobSeekerService.GetEmployerByIdAsync(employerId);
            return Ok(new { message = "Employer details retrieved successfully.", data = employer });
        }
        [HttpPost("UploadResume")]
        public async Task<IActionResult> UploadResume([FromForm] UploadResumeDto uploadDto)
        {
            if (uploadDto?.Resume == null)
                return BadRequest(new { message = "Resume file is required." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _jobSeekerService.UploadResumeAsync(jobSeekerId, uploadDto);
            return Ok(new { message = "Resume uploaded successfully." });
        }

        [HttpDelete("DeleteResume/{resumeId}")]
        public async Task<IActionResult> DeleteResume(int resumeId)
        {
            if (resumeId <= 0)
                return BadRequest(new { message = "Invalid resume ID." });

            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _jobSeekerService.DeleteResumeAsync(jobSeekerId, resumeId);
            return Ok(new { message = "Resume deleted successfully." });
        }

        [HttpGet("GetResumes")]
        public async Task<IActionResult> GetResumes()
        {
            var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var resumes = await _jobSeekerService.GetResumesAsync(jobSeekerId);
            if (resumes == null || !resumes.Any())
                return Ok(new { message = "No resumes found.", data = new List<object>() });

            return Ok(new { message = "Resumes retrieved successfully.", data = resumes });
        }
    }
}