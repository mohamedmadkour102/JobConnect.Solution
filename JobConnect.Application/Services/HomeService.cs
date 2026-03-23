using JobConnect.Application.Abstractions;
using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Application.DTOs;
using JobConnect.Application.DTOs.Admin;
using JobConnect.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace JobConnect.Application.Services;

public class HomeService : IHomeService
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeService> _logger;

    public HomeService(IUnitOfWork unitOfWork, IEmailService emailService, ILogger<HomeService> logger)
    {
        _uow = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SubmitContactAsync(ContactMessageDto dto, CancellationToken cancellationToken = default)
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

        await _uow.Home.AddContactMessageAsync(message, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var emailBody = $@"
                    <h2>New Contact Us Message</h2>
                    <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>
                        <tr>
                            <th style='background-color: #f2f2f2;'>Field</th>
                            <th style='background-color: #f2f2f2;'>Value</th>
                        </tr>
                        <tr><td>First Name</td><td>{dto.FirstName}</td></tr>
                        <tr><td>Last Name</td><td>{dto.LastName}</td></tr>
                        <tr><td>Phone</td><td>{dto.Phone}</td></tr>
                        <tr><td>Email</td><td>{dto.Email}</td></tr>
                        <tr><td>Message</td><td>{dto.Message}</td></tr>
                    </table>
                    <p><em>Message sent on: {DateTime.UtcNow:f}</em></p>
                ";

        await _emailService.SendEmailAsync("Mohamed.Madkour2002@gmail.com", "New Contact Us Message", emailBody);
        _logger.LogInformation("Contact Us message saved and email sent successfully for {Email}", dto.Email);
    }

    public async Task<List<ContactMessage>> GetAllMessagesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all Contact Us messages");
        return await _uow.Home.GetContactMessagesOrderedAsync(cancellationToken);
    }

    public async Task<List<string>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all Job Tags");
        var tags = await _uow.Home.GetDistinctJobTagsAsync(cancellationToken);
        if (tags.Count == 0)
            _logger.LogWarning("No Job Tags found in the database");
        return tags;
    }

    public async Task<object> GetJobsAsync(
        IReadOnlyDictionary<string, string>? filters,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching jobs with filters");

        var (pagedJobs, totalCount) = await _uow.Home.GetJobsWithFiltersAsync(filters, pageNumber, pageSize, cancellationToken);

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
            Employer = j.Employer == null
                ? null
                : new
                {
                    j.Employer.Id,
                    j.Employer.CompanyName,
                    j.Employer.Email,
                    j.Employer.PhoneNumber,
                    j.Employer.Industry
                }
        }).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new
        {
            message = result.Any() ? "Jobs retrieved successfully." : "No jobs found with the applied filters.",
            totalCount,
            totalPages,
            pageNumber,
            pageSize,
            data = result
        };
    }

    public async Task<object> GetJobsByTagAndIdAsync(string tag, int tagId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching jobs for tag: {Tag} and tagId: {TagId}", tag, tagId);

        var jobs = await _uow.Home.GetJobsByTagAndIdAsync(tag, tagId, cancellationToken);

        if (jobs.Count == 0)
        {
            _logger.LogWarning("No jobs found for tag: {Tag} and tagId: {TagId}", tag, tagId);
            return new { message = $"No jobs found for tag {tag} and tagId {tagId}.", data = new List<object>() };
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
            EmployerName = j.Employer!.CompanyName
        }).ToList();

        return new { message = $"Jobs for tag {tag} and tagId {tagId} retrieved successfully.", data = result };
    }

    private static string GetTimeAgo(DateTime date)
    {
        var timeSpan = DateTime.UtcNow - date;
        if (timeSpan.TotalDays >= 7)
            return $"{(int)timeSpan.TotalDays / 7} week ago";
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} days ago";
        return "Today";
    }

    private static int CalculateDaysRemaining(DateTime expirationDate)
    {
        var daysRemaining = (expirationDate - DateTime.UtcNow).Days;
        return daysRemaining > 0 ? daysRemaining : 0;
    }
}
