#region OldCode
//using JobConnect.Apis.IService;
//using JobConnect.Apis.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace JobConnect.Apis.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	[Authorize(Roles = "admin")]
//	public class AdminController : ControllerBase
//	{
//		private readonly IAdminService _adminService;

//		public AdminController(IAdminService adminService)
//		{
//			_adminService = adminService;
//		}

//		[HttpGet("employers")]
//		public async Task<IActionResult> GetAllEmployers()
//		{
//			var employers = await _adminService.GetAllEmployersAsync();
//			if (employers == null || !employers.Any())
//				return NotFound(new { message = "No employers found." });

//			return Ok(new { message = "Employers retrieved successfully.", data = employers });
//		}

//		[HttpGet("jobseekers")]
//		public async Task<IActionResult> GetAllJobSeekers()
//		{
//			var jobSeekers = await _adminService.GetAllJobSeekersAsync();
//			if (jobSeekers == null || !jobSeekers.Any())
//				return NotFound(new { message = "No job seekers found." });

//			return Ok(new { message = "Job seekers retrieved successfully.", data = jobSeekers });
//		}

//		[HttpDelete("user/{userId}")]
//		public async Task<IActionResult> DeleteUser(string userId)
//		{
//			try
//			{
//				var result = await _adminService.DeleteUserAsync(userId);
//				if (!result)
//					return NotFound(new { message = $"User with ID {userId} not found." });

//				return Ok(new { message = "User deleted successfully." });
//			}
//			catch (InvalidOperationException ex)
//			{
//				return BadRequest(new { message = ex.Message });
//			}
//		}

//		[HttpGet("jobs")]
//		public async Task<IActionResult> GetAllJobs()
//		{
//			var jobs = await _adminService.GetAllJobsAsync();
//			if (jobs == null || !jobs.Any())
//				return NotFound(new { message = "No jobs found." });

//			return Ok(new { message = "Jobs retrieved successfully.", data = jobs });
//		}

//		[HttpGet("jobs-by-tag/{tag}")]
//		public async Task<IActionResult> GetJobsByTag(string tag)
//		{
//			var jobs = await _adminService.GetJobsByTagAsync(tag);
//			if (jobs == null || !jobs.Any())
//				return NotFound(new { message = $"No jobs found for tag {tag}." });

//			return Ok(new { message = $"Jobs for tag {tag} retrieved successfully.", data = jobs });
//		}
//	}
//} 
#endregion

using JobConnect.Apis.DTO_s.Admin;
using JobConnect.Apis.IService;
using JobConnect.Apis.Services;
using JobConnect.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "admin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminService _adminService;
        private readonly INotificationService _notificationService;

        public AdminController(IAdminService adminService , INotificationService notificationService)
		{
			_adminService = adminService;
            _notificationService = notificationService;
        }

		[HttpGet("employers")]
		public async Task<IActionResult> GetAllEmployers()
		{
			var employers = await _adminService.GetAllEmployersAsync();
			if (employers == null || !employers.Any())
				return Ok(new { message = "No employers found.", data = new List<object>() });

			return Ok(new { message = "Employers retrieved successfully.", data = employers });
		}

		[HttpGet("jobseekers")]
		public async Task<IActionResult> GetAllJobSeekers()
		{
			var jobSeekers = await _adminService.GetAllJobSeekersAsync();
			if (jobSeekers == null || !jobSeekers.Any())
				return Ok(new { message = "No job seekers found.", data = new List<object>() });

			return Ok(new { message = "Job seekers retrieved successfully.", data = jobSeekers });
		}

		[HttpDelete("user/{userId}")]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			try
			{
				var result = await _adminService.DeleteUserAsync(userId);
				if (!result)
					return NotFound(new { message = $"User with ID {userId} not found." });

				return Ok(new { message = "User deleted successfully." });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet("jobs")]
		public async Task<IActionResult> GetAllJobs()
		{
			var jobs = await _adminService.GetAllJobsAsync();
			if (jobs == null || !jobs.Any())
				return Ok(new { message = "No jobs found.", data = new List<object>() });

			return Ok(new { message = "Jobs retrieved successfully.", data = jobs });
		}

		[HttpGet("jobs-by-tag/{tag}")]
		public async Task<IActionResult> GetJobsByTag(string tag)
		{
			var jobs = await _adminService.GetJobsByTagAsync(tag);
			if (jobs == null || !jobs.Any())
				return Ok(new { message = $"No jobs found for tag {tag}.", data = new List<object>() });

			return Ok(new { message = $"Jobs for tag {tag} retrieved successfully.", data = jobs });
		}
        [HttpPost("application/{applicationId}/status")]
        public async Task<IActionResult> UpdateApplicationStatus(string applicationId, [FromBody] UpdateApplicationStatusRequest request)
        {
            // Assume you have a method in _adminService to update application status
            var application = await _adminService.UpdateApplicationStatusAsync(applicationId, request.Status);
            if (application == null)
                return NotFound(new { message = $"Application with ID {applicationId} not found." });

            // Send notification to JobSeeker
            var notification = new Notification
            {
                Title = "Application Status Update",
                Message = $"Your application for job ID {application.JobId} has been {request.Status}.",
                Redirect = $"app/jobs/{application.JobId}",
                Type = "application_status"
            };

            await _notificationService.SendNotificationAsync(application.JobSeekerId, notification);

            return Ok(new { message = "Application status updated and notification sent." });
        }
    }
}