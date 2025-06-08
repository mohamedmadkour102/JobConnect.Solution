using JobConnect.Apis.DTO_s;
using JobConnect.Core.Models;
using JobConnect.Core.Services;
using JobConnect.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobConnect.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, IEmailService emailService, ILogger<HomeController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("ContactUs")]
        public async Task<IActionResult> ContactUs([FromBody] ContactMessageDto dto)
        {
            try
            {
                _logger.LogInformation("Received Contact Us message from {Email}", dto.Email);

                var message = new ContactMessage
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Message = dto.Message
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                var emailBody = $@"
                    <h2>New Contact Us Message</h2>
                    <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>
                        <tr>
                            <th style='background-color: #f2f2f2;'>Field</th>
                            <th style='background-color: #f2f2f2;'>Value</th>
                        </tr>
                        <tr>
                            <td>First Name</td>
                            <td>{dto.FirstName}</td>
                        </tr>
                        <tr>
                            <td>Last Name</td>
                            <td>{dto.LastName}</td>
                        </tr>
                        <tr>
                            <td>Phone</td>
                            <td>{dto.Phone}</td>
                        </tr>
                        <tr>
                            <td>Email</td>
                            <td>{dto.Email}</td>
                        </tr>
                        <tr>
                            <td>Message</td>
                            <td>{dto.Message}</td>
                        </tr>
                    </table>
                    <p><em>Message sent on: {DateTime.UtcNow.ToString("f")}</em></p>
                ";

                await _emailService.SendEmailAsync("Mohamed.Madkour2002@gmail.com", "New Contact Us Message", emailBody);

                _logger.LogInformation("Contact Us message saved and email sent successfully for {Email}", dto.Email);
                return Ok(new { Message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Contact Us message from {Email}", dto.Email);
                return StatusCode(500, new { Message = "An error occurred while processing your message." });
            }
        }

        [HttpGet("GetAllMessages")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all Contact Us messages");

                var messages = await _context.ContactMessages
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Contact Us messages");
                return StatusCode(500, new { Message = "An error occurred while fetching messages." });
            }
        }

        [HttpGet("GetAllTags")]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                _logger.LogInformation("Fetching all Job Tags");

                var tags = await _context.JobTags
                    .Select(jt => jt.Tag)
                    .Distinct()
                    .ToListAsync();

                if (!tags.Any())
                {
                    _logger.LogWarning("No Job Tags found in the database");
                    return NotFound(new { Message = "No tags found." });
                }

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Job Tags");
                return StatusCode(500, new { Message = "An error occurred while fetching tags." });
            }
        }

        //Test {
//  "title": "Default Job",
//  "location": "Remote",
//  "experience": "2-5 years",
//  "minSalary": 50000
//}

    [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs([FromQuery] Dictionary<string, string> filters)
        {
            try
            {
                _logger.LogInformation("Fetching all jobs with filters: {Filters}", string.Join(", ", filters.Select(f => $"{f.Key}={f.Value}")));

                var query = _context.Jobs
                    .AsQueryable();

                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        switch (filter.Key.ToLower())
                        {
                            case "searchterm":
                                query = query.Where(j => j.Title.Contains(filter.Value) ||
                                                      j.Location.Contains(filter.Value));
                                break;
                            case "experience":
                                query = query.Where(j => j.Experience == filter.Value);
                                break;
                            case "minsalary":
                                if (decimal.TryParse(filter.Value, out decimal minSalary))
                                {
                                    query = query.Where(j => j.MinSalary >= minSalary);
                                }
                                break;
                            case "maxsalary":
                                if (decimal.TryParse(filter.Value, out decimal maxSalary))
                                {
                                    query = query.Where(j => j.MaxSalary <= maxSalary);
                                }
                                break;
                            case "jobtype":
                                query = query.Where(j => j.JobType == filter.Value);
                                break;
                            case "educationlevel":
                                query = query.Where(j => j.Education == filter.Value);
                                break;
                            case "location":
                                query = query.Where(j => j.Location.Contains(filter.Value));
                                break;
                        }
                    }
                }

                var jobs = await query
                    .Select(j => new
                    {
                        j.Id,
                        j.Description,
                        j.MinSalary,
                        j.MaxSalary,
                        j.SalaryType,
                        j.Education,
                        j.Experience,
                        j.Vacancies,
                        j.ExpirationDate,
                        j.Title,
                        j.Status,
                        j.ApplicationCount,
                        j.JobType,
                        j.WorkPlace,
                        j.DaysRemaining,
                        j.PostedDate,
                        j.Location
                    })
                    .ToListAsync();

                if (!jobs.Any())
                {
                    _logger.LogWarning("No jobs found with the applied filters");
                    return NotFound(new { Message = "No jobs found." });
                }

                _logger.LogInformation("Successfully fetched {Count} jobs", jobs.Count);
                return Ok(new { Message = "Jobs retrieved successfully.", Data = jobs });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs with filters");
                return StatusCode(500, new { Message = "An error occurred while fetching jobs." });
            }
        }
    }
}