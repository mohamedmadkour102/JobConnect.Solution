using JobConnect.Apis.DTO_s.EmployerDto;
using JobConnect.Apis.DTO_s;
using JobConnect.Apis.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace JobConnect.Apis.Controllers
{
	[Route("api/employer")]
	[ApiController]
	//[Authorize(Roles = "employer")]
	public class EmployerController : ControllerBase
	{
		private readonly IJobService _jobService;

		public EmployerController(IJobService jobService)
		{
			_jobService = jobService;
		}

		[HttpGet("GetRecentJobs")]
		public async Task<IActionResult> GetRecentJobs()
		{
			var jobs = await _jobService.GetRecentJobsAsync();
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No recent jobs found." });

			return Ok(new { message = "Recent jobs retrieved successfully.", data = jobs });
		}
		

		[HttpGet("GetAllJobs")]
		public async Task<IActionResult> GetAllJobs()
		{
			var jobs = await _jobService.GetAllJobsAsync();
			if (jobs == null || !jobs.Any())
				return NotFound(new { message = "No jobs available at the moment." });

			return Ok(new { message = "All jobs retrieved successfully.", data = jobs });
		}

		[HttpGet("GetJobById")]
		public async Task<IActionResult> GetJobById(int id)
		{
			var job = await _jobService.GetJobByIdAsync(id);
			if (job == null)
				return NotFound(new { message = $"Job with ID {id} not found." });

			return Ok(new { message = "Job details retrieved successfully.", data = job });
		}

		[HttpPost("PostJob")]
		public async Task<IActionResult> PostJob([FromBody] CreateJobDto jobDto)
		{
			if (jobDto == null)
				return BadRequest(new { message = "Invalid job data provided." });

			await _jobService.AddJobAsync(jobDto);
			return CreatedAtAction(nameof(GetAllJobs), new { message = "Job created successfully." });
		}

		[HttpPut("UpdateJob")]
		public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobDto jobDto)
		{
			if (jobDto == null)
				return BadRequest(new { message = "Invalid job data provided." });

			var jobExists = await _jobService.GetJobByIdAsync(id);
			if (jobExists == null)
				return NotFound(new { message = $"Job with ID {id} not found for update." });

			await _jobService.UpdateJobAsync(id, jobDto);
			return Ok(new { message = "Job updated successfully." });
		}

		[HttpDelete("DeleteJob")]
		public async Task<IActionResult> DeleteJob(int id)
		{
			var jobExists = await _jobService.GetJobByIdAsync(id);
			if (jobExists == null)
				return NotFound(new { message = $"Job with ID {id} not found for deletion." });

			await _jobService.DeleteJobAsync(id);
			return Ok(new { message = "Job deleted successfully." });
		}

		[HttpGet("GetJobStats")]
		public async Task<IActionResult> GetJobStats()
		{
			var stats = await _jobService.GetJobStatsAsync();
			if (stats == null)
				return NotFound(new { message = "No job statistics available at the moment." });

			return Ok(new { message = "Job statistics retrieved successfully.", data = stats });
		}
	}
}
