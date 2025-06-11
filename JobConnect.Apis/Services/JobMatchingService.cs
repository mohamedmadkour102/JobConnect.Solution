using JobConnect.Apis.Models;
using JobConnect.Core.IService;
using JobConnect.Core.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace JobConnect.Core.Services
{
    public class JobMatchingService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public JobMatchingService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task MatchJobWithSeekers(Job job)
        {
            // جلب كل الباحثين عن عمل
            var jobSeekers = await _context.JobSeekers
                .Include(js => js.Skills)
                .Include(js => js.WorkedAs)
                .ToListAsync();

            foreach (var seeker in jobSeekers)
            {
                // التحقق من تطابق المهارات
                var hasMatchingSkills = job.Tags.Any(tag => 
                    seeker.Skills.Any(skill => 
                        skill.SkillName.ToLower() == tag.Tag.ToLower()));

                // التحقق من تطابق الخبرة
                var hasMatchingExperience = seeker.YearsOfExperience >= 
                    int.Parse(job.Experience.Split(' ')[0]);

                // التحقق من تطابق الوظيفة الحالية/المطلوبة
                var hasMatchingJob = seeker.WorkedAs.Any(work => 
                    work.JobTitle.ToLower() == job.Title.ToLower());

                if (hasMatchingSkills || hasMatchingExperience || hasMatchingJob)
                {
                    // إرسال إشعار للمستخدم
                    await _notificationService.SendNotificationToUserAsync(
                        seeker.Id,
                        new Notification
                        {
                            Title = "وظيفة مناسبة ليك",
                            Message = $"تم نشر وظيفة {job.Title} مناسبة لمهاراتك وخبراتك",
                            Type = NotificationType.JobMatch,
                            DataJson = JsonSerializer.Serialize(new { jobId = job.Id }),
                            RedirectUrl = $"/jobs/{job.Id}"
                        }
                    );
                }
            }
        }
    }
} 