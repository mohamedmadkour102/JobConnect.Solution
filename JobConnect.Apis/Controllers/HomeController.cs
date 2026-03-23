using JobConnect.Application.Abstractions;
using JobConnect.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JobConnect.Apis.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IHomeService _homeService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IHomeService homeService, ILogger<HomeController> logger)
    {
        _homeService = homeService;
        _logger = logger;
    }

    [HttpPost("ContactUs")]
    public async Task<IActionResult> ContactUs([FromBody] ContactMessageDto dto)
    {
        try
        {
            await _homeService.SubmitContactAsync(dto);
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
            var messages = await _homeService.GetAllMessagesAsync();
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
            var tags = await _homeService.GetAllTagsAsync();
            if (tags.Count == 0)
                return NotFound(new { Message = "No tags found." });
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
            var result = await _homeService.GetJobsAsync(filters, pageNumber, pageSize);
            return Ok(result);
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
            var result = await _homeService.GetJobsByTagAndIdAsync(tag, tagId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching jobs for tag: {Tag} and tagId: {TagId}", tag, tagId);
            return StatusCode(500, new { Message = "An error occurred while fetching jobs." });
        }
    }
}
