using JobConnect.Apis.DTO_s;
using JobConnect.Apis.DTO_s.Admin;
using JobConnect.Core.Models;
using JobConnect.Core.Services;
using JobConnect.Repository.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        private string GetTimeAgo(DateTime date)
        {
            TimeSpan timeSpan = DateTime.UtcNow - date;
            if (timeSpan.TotalDays >= 7)
                return $"{(int)timeSpan.TotalDays / 7} week ago";
            else if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            return "Today";
        }

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

        [HttpGet("GetAllJobs")]
        public async Task<IActionResult> GetAllJobs(
            [FromQuery] Dictionary<string, string> filters,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all jobs with filters: {Filters}", string.Join(", ", filters.Select(f => $"{f.Key}={f.Value}")));

                var query = _context.Jobs
                    .Include(j => j.Employer)
                    .AsQueryable();

                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        var key = filter.Key.ToLower();
                        var value = filter.Value?.Trim();

                        if (string.IsNullOrWhiteSpace(value) || value.ToLower() == "")
                            continue;

                        switch (key)
                        {
                            case "searchterm":
                            case "title":
                                query = query.Where(j => j.Title.ToLower().Contains(value.ToLower()) ||
                                                         j.Location.ToLower().Contains(value.ToLower()));
                                break;

                            case "minsalary":
                                if (decimal.TryParse(value, out decimal minSalary))
                                {
                                    query = query.Where(j => j.MinSalary >= minSalary);
                                }
                                break;

                            case "maxsalary":
                                if (decimal.TryParse(value, out decimal maxSalary))
                                {
                                    query = query.Where(j => j.MaxSalary <= maxSalary);
                                }
                                break;

                            case "jobtype":
                                query = query.Where(j => j.JobType.ToLower() == value.ToLower());
                                break;

                            case "educationlevel":
                                query = query.Where(j => j.Education.ToLower() == value.ToLower());
                                break;

                            case "location":
                                query = query.Where(j => j.Location.ToLower().Contains(value.ToLower()));
                                break;

                            case "workplace":
                                query = query.Where(j => j.WorkPlace.ToLower() == value.ToLower());
                                break;
                        }
                    }
                }

                var jobs = await query.ToListAsync();

                // Now apply experience filter in-memory
                if (filters != null && filters.TryGetValue("experience", out var expFilterValue))
                {
                    expFilterValue = expFilterValue?.Trim();
                    if (!string.IsNullOrWhiteSpace(expFilterValue) && expFilterValue.ToLower() != "all")
                    {
                        jobs = jobs.Where(j =>
                        {
                            if (!int.TryParse(j.Experience, out var exp))
                                return false;

                            return expFilterValue switch
                            {
                                "0" => exp == 0,
                                "0-2" => exp >= 0 && exp <= 2,
                                "2-5" => exp >= 2 && exp <= 5,
                                "5-8" => exp >= 5 && exp <= 8,
                                "8+" => exp >= 8,
                                _ => true
                            };
                        }).ToList();
                    }
                }

                var totalCount = jobs.Count;

                var pagedJobs = jobs
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();


                var result = pagedJobs.Select(j => new
                {
                    j.Id,
                    j.Title,
                    j.Description,
                    j.MinSalary,
                    j.MaxSalary,
                    j.SalaryType,
                    j.Education,
                    j.Experience,
                    j.Vacancies,
                    ExpirationDate = j.ExpirationDate.ToString("yyyy-MM-dd"),
                    PostedDate = GetTimeAgo(j.PostedDate),
                    j.Status,
                    j.ApplicationCount,
                    j.JobType,
                    j.WorkPlace,
                    j.DaysRemaining,
                    j.Location,

                    Employer = j.Employer == null ? null : new
                    {
                        j.Employer.Id,
                        j.Employer.CompanyName,
                        j.Employer.Email,
                        j.Employer.PhoneNumber,
                        j.Employer.Industry
                    }
                }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                _logger.LogInformation("Successfully fetched {Count} jobs (Page {PageNumber} with size {PageSize})", result.Count, pageNumber, pageSize);

                return Ok(new
                {
                    message = result.Any() ? "Jobs retrieved successfully." : "No jobs found with the applied filters.",
                    totalCount = totalCount,
                    totalPages = totalPages,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs with filters");
                return StatusCode(500, new { Message = "An error occurred while fetching jobs." });
            }
        }

        [HttpGet("GetJobsByTags/{tag}/{tagId}")]
        public async Task<IActionResult> GetJobsByTagAndId(string tag, int tagId)
        {
            try
            {
                _logger.LogInformation("Fetching jobs for tag: {Tag} and tagId: {TagId}", tag, tagId);

                var jobs = await _context.Jobs
                    .Include(j => j.Employer)
                    .Include(j => j.Tags)
                    .Include(j => j.Responsibilities)
                    .Include(j => j.Applications)
                    .Where(j => j.Tags.Any(t => t.Tag == tag && t.Id == tagId))
                    .ToListAsync();

                if (jobs == null || !jobs.Any())
                {
                    _logger.LogWarning("No jobs found for tag: {Tag} and tagId: {TagId}", tag, tagId);
                    return Ok(new { message = $"No jobs found for tag {tag} and tagId {tagId}.", data = new List<object>() });
                }

                var result = jobs.Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Status = j.Status,
                    ApplicationsCount = j.Applications.Count,
                    JobType = j.JobType,
                    DaysRemaining = CalculateDaysRemaining(j.ExpirationDate),
                    PostedDate = GetTimeAgo(j.PostedDate),
                    Location = j.Location,
                    Tags = j.Tags.Select(t => t.Tag).ToList(),
                    Responsibilities = j.Responsibilities.Select(r => r.Responsibility).ToList(),
                    EmployerName = j.Employer.CompanyName
                }).ToList();

                _logger.LogInformation("Successfully retrieved {Count} jobs for tag: {Tag} and tagId: {TagId}", result.Count, tag, tagId);
                return Ok(new { message = $"Jobs for tag {tag} and tagId {tagId} retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs for tag: {Tag} and tagId: {TagId}", tag, tagId);
                return StatusCode(500, new { Message = "An error occurred while fetching jobs." });
            }
        }


        [HttpGet("GetJobById/{jobId}")]
        public async Task<IActionResult> GetJobById(int jobId)
        {
            try
            {
                _logger.LogInformation("Fetching job details for ID: {JobId}", jobId);
                
                var job = await _context.Jobs
                    .Include(j => j.Applications)
                    .Include(j => j.Tags)
                    .Include(j => j.Responsibilities)
                    .Include(j => j.Employer)
                    .FirstOrDefaultAsync(j => j.Id == jobId);
                
                if (job == null)
                {
                    _logger.LogWarning("Job with ID {JobId} not found", jobId);
                    return NotFound(new { message = $"Job with ID {jobId} not found." });
                }

                var jobDto = new JobConnect.Apis.DTO_s.SeekerDto.JobDto
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
                    Employer = new JobConnect.Apis.DTO_s.SeekerDto.EmployerInfo
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

                _logger.LogInformation("Successfully retrieved job details for ID: {JobId}", jobId);
                return Ok(new { message = "Job details retrieved successfully.", data = jobDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching job details for ID: {JobId}", jobId);
                return StatusCode(500, new { message = "An error occurred while retrieving job details." });
            }
        }

        private int CalculateDaysRemaining(DateTime expirationDate)
        {
            int daysRemaining = (expirationDate - DateTime.UtcNow).Days;
            return daysRemaining > 0 ? daysRemaining : 0;
        }

    }
}