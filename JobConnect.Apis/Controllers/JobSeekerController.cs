using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
	[Route("api/jobseeker")]
	[ApiController]
	[Authorize]
	public class JobSeekerController : ControllerBase
	{
		private readonly IJobSeekerService _jobSeekerService;

		public JobSeekerController(IJobSeekerService jobSeekerService)
		{
			_jobSeekerService = jobSeekerService;
		}

		[HttpGet("GetSavedJobs")]
		public async Task<IActionResult> GetSavedJobs()
		{
			var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var jobs = await _jobSeekerService.GetSavedJobsAsync(jobSeekerId);
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No saved jobs found." });

			return Ok(new { message = "Saved jobs retrieved successfully.", data = jobs });
		}

		[HttpPost("SaveJob")]
		public async Task<IActionResult> SaveJob([FromBody] int jobId)
		{
			var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			await _jobSeekerService.SaveJobAsync(jobSeekerId, jobId);
			return Ok(new { message = "Job saved successfully." });
		}

		[HttpPost("UnsaveJob")]
		public async Task<IActionResult> UnsaveJob([FromBody] int jobId)
		{
			var jobSeekerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			await _jobSeekerService.UnsaveJobAsync(jobSeekerId, jobId);
			return Ok(new { message = "Job unsaved successfully." });
		}

		[HttpGet("GetAllJobs")]
		public async Task<IActionResult> GetAllJobs()
		{
			var jobs = await _jobSeekerService.GetAllJobsAsync();
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No jobs available." });

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
	}
}