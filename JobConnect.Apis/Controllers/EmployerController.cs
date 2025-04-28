using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
	[Route("api/employer")]
	[ApiController]
	[Authorize]
	public class EmployerController : ControllerBase
	{
		private readonly IEmployerService _employerService;

		public EmployerController(IEmployerService employerService)
		{
			_employerService = employerService;
		}

		[HttpGet("GetRecentJobs")]
		public async Task<IActionResult> GetRecentJobs()
		{
			var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var jobs = await _employerService.GetRecentJobsAsync(employerId);
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No recent jobs found." });

			return Ok(new { message = "Recent jobs retrieved successfully.", data = jobs });
		}

		[HttpGet("GetAllJobs")]
		public async Task<IActionResult> GetAllJobs()
		{
			var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var jobs = await _employerService.GetJobsByEmployerAsync(employerId);
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No jobs available at the moment." });

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
				return NotFound(new { message = "No job statistics available at the moment." });

			return Ok(new { message = "Job statistics retrieved successfully.", data = stats });
		}

		[HttpGet("GetCompanyInfo")]
		public async Task<IActionResult> GetCompanyInfo()
		{
			var employerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var employer = await _employerService.GetEmployerByIdAsync(employerId);
			if (employer == null)
				return NotFound(new { message = "Employer not found." });

			var companyInfo = new
			{
				employer.CompanyName,
				employer.CompanyDescription,
				employer.LogoUrl
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
	}
}