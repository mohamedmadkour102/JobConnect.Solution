using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobConnect.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployerController : ControllerBase
    {
        private readonly IEmployerService _employerService;
        private readonly IWebHostEnvironment _environment;

        public EmployerController(IEmployerService employerService, IWebHostEnvironment environment)
        {
            _employerService = employerService;
            _environment = environment;
        }

        [HttpGet("GetRecentJobs")]
        public async Task<IActionResult> GetRecentJobs()
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobs = await _employerService.GetRecentJobsAsync(employerId);
            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No recent jobs found.", data = new List<object>() });

            return Ok(new { message = "Recent jobs retrieved successfully.", data = jobs });
        }

        [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobs = await _employerService.GetJobsByEmployerAsync(employerId);
            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No jobs available at the moment.", data = new List<object>() });

            return Ok(new { message = "All jobs retrieved successfully.", data = jobs });
        }

        [HttpGet("GetJobById")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var job = await _employerService.GetJobByIdAsync(id, employerId);
            if (job == null)
                return NotFound(new { message = $"Job with ID {id} not found." });

            return Ok(new { message = "Job details retrieved successfully.", data = job });
        }

        [HttpPost("PostJob")]
        public async Task<IActionResult> PostJob([FromBody] CreateJobDto jobDto)
        {
            if (jobDto == null)
                return BadRequest(new { message = "Invalid job data provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _employerService.AddJobAsync(employerId, jobDto);
            return CreatedAtAction(nameof(GetAllJobs), new { message = "Job created successfully." });
        }

        [HttpPut("UpdateJob")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobDto jobDto)
        {
            if (jobDto == null)
                return BadRequest(new { message = "Invalid job data provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobExists = await _employerService.GetJobByIdAsync(id, employerId);
            if (jobExists == null)
                return NotFound(new { message = $"Job with ID {id} not found for update." });

            await _employerService.UpdateJobAsync(id, employerId, jobDto);
            return Ok(new { message = "Job updated successfully." });
        }

        [HttpDelete("DeleteJob")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobExists = await _employerService.GetJobByIdAsync(id, employerId);
            if (jobExists == null)
                return NotFound(new { message = $"Job with ID {id} not found for deletion." });

            await _employerService.DeleteJobAsync(id, employerId);
            return Ok(new { message = "Job deleted successfully." });
        }

        [HttpGet("GetJobStats")]
        public async Task<IActionResult> GetJobStats()
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var stats = await _employerService.GetJobStatsAsync(employerId);
            if (stats == null)
                return Ok(new { message = "No job statistics available at the moment.", data = (object)null });

            return Ok(new { message = "Job statistics retrieved successfully.", data = stats });
        }

        [HttpGet("GetCompanyInfo")]
        public async Task<IActionResult> GetCompanyInfo()
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = await _employerService.GetEmployerByIdAsync(employerId);
            if (employer == null)
                return NotFound(new { message = "Employer not found." });

            var companyInfo = new CompanyInfoDto
            {
                CompanyName = employer.CompanyName,
                CompanyDescription = employer.CompanyDescription,
                LogoUrl = employer.LogoUrl
            };

            return Ok(new { message = "Company info retrieved successfully.", data = companyInfo });
        }

        [HttpPut("UpdateCompanyInfo")]
        public async Task<IActionResult> UpdateCompanyInfo([FromForm] UpdateCompanyInfoDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid company info provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _employerService.UpdateCompanyInfoAsync(employerId, dto);

            return Ok(new { message = "Company info updated successfully." });
        }

        [HttpGet("GetFoundingInfo")]
        public async Task<IActionResult> GetFoundingInfo()
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employer = await _employerService.GetEmployerByIdAsync(employerId);
            if (employer == null)
                return NotFound(new { message = "Employer not found." });

            var foundingInfo = new
            {
                employer.Industry,
                employer.CompanySize,
                employer.FoundingDate,
                employer.Website,
                employer.PhoneNumber
            };

            return Ok(new { message = "Founding info retrieved successfully.", data = foundingInfo });
        }

        [HttpPut("UpdateFoundingInfo")]
        public async Task<IActionResult> UpdateFoundingInfo([FromBody] UpdateFoundingInfoDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid founding info provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _employerService.UpdateFoundingInfoAsync(employerId, dto);

            return Ok(new { message = "Founding info updated successfully." });
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Invalid password data provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _employerService.ChangePasswordAsync(employerId, dto);

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpGet("GetAllJobsPaginated")]
        public async Task<IActionResult> GetAllJobsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { message = "Page number and page size must be greater than 0." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var (jobs, totalCount) = await _employerService.GetJobsByEmployerPaginatedAsync(employerId, pageNumber, pageSize);

            if (jobs == null || !jobs.Any())
                return Ok(new { message = "No jobs available at the moment.", data = new List<object>(), totalCount = 0 });

            return Ok(new
            {
                message = "All jobs retrieved successfully with pagination.",
                data = jobs,
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employerId))
                    return Unauthorized(new { message = "Invalid user." });

                await _employerService.DeleteEmployerAccountAsync(employerId);
                return Ok(new { message = "Account deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddToShortlist")] 
        public async Task<IActionResult> AddToShortlist([FromBody] ShortlistDto shortlistDto)
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var job = await _employerService.GetJobByIdAsync(shortlistDto.JobId, employerId);
            if (job == null)
                return NotFound(new { message = "Job not found or you don't have access to it." });

            await _employerService.AddToShortlistAsync(shortlistDto.JobId, shortlistDto.JobSeekerId);
            return Ok(new { message = "JobSeeker added to shortlist successfully." });
        }

        [HttpPost("RemoveFromShortlist")]
        public async Task<IActionResult> RemoveFromShortlist([FromBody] ShortlistDto shortlistDto)
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var job = await _employerService.GetJobByIdAsync(shortlistDto.JobId, employerId);
            if (job == null)
                return NotFound(new { message = "Job not found or you don't have access to it." });

            await _employerService.RemoveFromShortlistAsync(shortlistDto.JobId, shortlistDto.JobSeekerId);
            return Ok(new { message = "JobSeeker removed from shortlist successfully." });
        }

        [HttpGet("GetShortlistedJobSeekers/{jobId}")]
        public async Task<IActionResult> GetShortlistedJobSeekers(int jobId)
        {
            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var job = await _employerService.GetJobByIdAsync(jobId, employerId);
            if (job == null)
                return NotFound(new { message = "Job not found or you don't have access to it." });

            var shortlistedJobSeekers = await _employerService.GetShortlistedJobSeekersAsync(jobId, employerId);
            if (!shortlistedJobSeekers.Any())
                return NotFound(new { message = "No shortlisted job seekers found for this job." });

            return Ok(new { message = "Shortlisted job seekers retrieved successfully.", data = shortlistedJobSeekers });
        }


        [HttpGet("GetJobSeekerById/{jobSeekerId}")]
        public async Task<IActionResult> GetJobSeekerById(string jobSeekerId)
        {
            if (string.IsNullOrEmpty(jobSeekerId))
                return BadRequest(new { message = "Invalid JobSeeker ID." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jobSeeker = await _employerService.GetJobSeekerByIdAsync(employerId, jobSeekerId);

            if (jobSeeker == null)
                return NotFound(new { message = "JobSeeker not found or not associated with your jobs." });

            return Ok(new { message = "JobSeeker retrieved successfully.", data = jobSeeker });
        }

        [HttpGet("GetSeekerResumesWithId/{jobSeekerId}")]
        public async Task<IActionResult> GetSeekerResumesWithId(string jobSeekerId)
        {
            if (string.IsNullOrEmpty(jobSeekerId))
                return BadRequest(new { message = "Invalid JobSeeker ID." });

            var resumes = await _employerService.GetSeekerResumesWithIdAsync(jobSeekerId);

            if (resumes == null || !resumes.Any())
                return Ok(new { message = "No resumes found for this JobSeeker.", data = new List<object>() });

            return Ok(new { message = "Resumes retrieved successfully.", data = resumes });
        }

        [HttpGet("GetApplicantsWithResume/{jobId}")]
        public async Task<IActionResult> GetApplicantsWithResume(int jobId)
        {
            try
            {
                var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employerId))
                    return Unauthorized(new { message = "Invalid user." });

                var applicants = await _employerService.GetApplicantsWithResumeAsync(jobId, employerId);
                if (!applicants.Any())
                    return Ok(new { message = "No applicants found for this job.", data = new List<object>() });

                return Ok(new { message = "Applicants retrieved successfully.", data = applicants });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("Hire")]
        public async Task<IActionResult> Hire([FromBody] ApplicationActionDto dto)
        {
            if (dto == null || dto.JobId <= 0 || string.IsNullOrEmpty(dto.JobSeekerId))
                return BadRequest(new { message = "Invalid job or job seeker data provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _employerService.HireApplicantAsync(employerId, dto.JobId, dto.JobSeekerId);

            if (!success)
                return BadRequest(new { message = "Failed to hire applicant. Check if job has vacancies or application exists." });

            return Ok(new { message = "Applicant hired successfully." });
        }

        [HttpPost("Reject")]
        public async Task<IActionResult> Reject([FromBody] ApplicationActionDto dto)
        {
            if (dto == null || dto.JobId <= 0 || string.IsNullOrEmpty(dto.JobSeekerId))
                return BadRequest(new { message = "Invalid job or job seeker data provided." });

            var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _employerService.RejectApplicantAsync(employerId, dto.JobId, dto.JobSeekerId);

            if (!success)
                return BadRequest(new { message = "Failed to reject applicant. Check if application exists." });

            return Ok(new { message = "Applicant rejected successfully." });
        }
    }
}