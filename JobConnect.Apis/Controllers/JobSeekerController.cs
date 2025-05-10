using JobConnect.Apis.DTO_s.SeekerDto;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;
using JobConnect.Apis.Helpers;

namespace JobConnect.Apis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class JobSeekerController : ControllerBase
	{
		private readonly IJobSeekerService _jobSeekerService;
		private readonly IWebHostEnvironment _environment;

		public JobSeekerController(IJobSeekerService jobSeekerService, IWebHostEnvironment environment)
		{
			_jobSeekerService = jobSeekerService;
			_environment = environment;
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
			await _jobSeekerService.SaveJobAsync(jobSeekerId, request.JobId);
			return Ok(new { message = "Job saved successfully." });
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
	}
}