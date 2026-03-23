using System.Text.Json;
using JobConnect.Application.Abstractions;
using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;

namespace JobConnect.Infrastructure.Services;

public class JobMatchingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public JobMatchingService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task MatchJobWithSeekers(Job job)
    {
        var jobSeekers = await _unitOfWork.JobSeekers.GetJobSeekersForMatchingAsync();

        foreach (var seeker in jobSeekers)
        {
            var hasMatchingSkills = job.Tags.Any(tag =>
                seeker.Skills.Any(skill =>
                    string.Equals(skill.SkillName, tag.Tag, StringComparison.OrdinalIgnoreCase)));

            var expParts = job.Experience.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var minExp = expParts.Length > 0 && int.TryParse(expParts[0], out var parsed) ? parsed : 0;
            var hasMatchingExperience = seeker.YearsOfExperience >= minExp;

            var hasMatchingJob = seeker.WorkedAs.Any(work =>
                string.Equals(work.JobTitle, job.Title, StringComparison.OrdinalIgnoreCase));

            if (hasMatchingSkills || hasMatchingExperience || hasMatchingJob)
            {
                await _notificationService.SendNotificationToUserAsync(
                    seeker.Id,
                    new Notification
                    {
                        Title = "وظيفة مناسبة ليك",
                        Message = $"تم نشر وظيفة {job.Title} مناسبة لمهاراتك وخبراتك",
                        Type = NotificationType.JobMatch,
                        DataJson = JsonSerializer.Serialize(new { jobId = job.Id }),
                        RedirectUrl = $"/jobs/{job.Id}"
                    });
            }
        }
    }
}
