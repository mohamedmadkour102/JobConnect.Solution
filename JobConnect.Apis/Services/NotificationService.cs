using JobConnect.Core.IService;
using JobConnect.Core.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace JobConnect.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public NotificationService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task SendNotificationToUserAsync(string userId, Notification notification)
        {
            notification.UserId = userId;
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            var deviceToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.UserId == userId);

            if (deviceToken != null)
            {
                await SendPushNotificationAsync(
                    deviceToken.PushToken,
                    notification.Title,
                    notification.Message,
                    notification.Type,
                    notification.Data
                );
            }
        }

        public async Task MarkAsReadAsync(string notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Notification>> GetAllForUserAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task RegisterExpoTokenAsync(string userId, string expoToken)
        {
            var existingToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.UserId == userId);

            if (existingToken != null)
            {
                existingToken.PushToken = expoToken;
            }
            else
            {
                await _context.DeviceTokens.AddAsync(new DeviceToken
                {
                    UserId = userId,
                    PushToken = expoToken,
                    Platform = "android" // يمكن تحديثه لاحقاً بناءً على نوع الجهاز
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task SendPushNotificationAsync(string expoToken, string title, string body, NotificationType type, object data = null)
        {
            var message = new
            {
                to = expoToken,
                sound = "default",
                title,
                body,
                data = new
                {
                    type = type.ToString(),
                    redirectUrl = GetRedirectUrl(type, data),
                    data
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(message),
                Encoding.UTF8,
                "application/json"
            );

            await _httpClient.PostAsync(
                "https://exp.host/--/api/v2/push/send",
                content
            );
        }

        private string GetRedirectUrl(NotificationType type, object data)
        {
            if (data is JsonElement jsonElement)
            {
                switch (type)
                {
                    case NotificationType.ApplicationStatus:
                        if (jsonElement.TryGetProperty("ApplicationId", out var applicationId))
                        {
                            return $"applied/{applicationId.GetInt32()}";
                        }
                        break;

                    case NotificationType.Message:
                        if (jsonElement.TryGetProperty("MessageId", out var messageId))
                        {
                            return $"notifications?messageId={messageId.GetString()}";
                        }
                        break;

                    case NotificationType.Recommendation:
                        return "jobs";

                    case NotificationType.CompleteProfile:
                        return "profile";
                }

                return string.Empty;
            }

            // Fallback for when data is not JsonElement (like anonymous object in memory)
            switch (type)
            {
                case NotificationType.ApplicationStatus:
                    return $"applied/{((dynamic)data).ApplicationId}";

                case NotificationType.Message:
                    return $"notifications?messageId={((dynamic)data).MessageId}";

                case NotificationType.Recommendation:
                    return "jobs";

                case NotificationType.CompleteProfile:
                    return "profile";

                default:
                    return string.Empty;
            }
        }



        public async Task SendCompleteProfileReminderAsync(string userId)
        {
            var data = new { };
            var notification = BuildNotification(userId,
                "Complete Your Profile",
                "We need more details to match you with jobs.",
                NotificationType.CompleteProfile,
                data);

            await SendNotificationToUserAsync(userId, notification);
        }


        public async Task SendApplicationStatusNotification(string userId, int applicationId, string status)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null || application.Job == null)
                return;

            var jobTitle = application.Job.Title;

            var data = new { ApplicationId = applicationId };
            var notification = BuildNotification(userId,
                "Application Update",
                $"Your application for {jobTitle} has been {status.ToLower()}.",
                NotificationType.ApplicationStatus,
                data);

            await SendNotificationToUserAsync(userId, notification);
        }



        public async Task SendMessageNotification(string userId, string messageId, string senderName)
        {
            var data = new { MessageId = messageId };
            var notification = BuildNotification(userId,
                $"New message from {senderName}",
                "You have a new message",
                NotificationType.Message,
                data);

            await SendNotificationToUserAsync(userId, notification);
        }


        public async Task SendRecommendationNotification(string userId, string recommendationText)
        {
            var data = new { };
            var notification = BuildNotification(userId,
                "New Recommendation",
                recommendationText,
                NotificationType.Recommendation,
                data);

            await SendNotificationToUserAsync(userId, notification);
        }

        private Notification BuildNotification(string userId, string title, string message, NotificationType type, object data)
        {
            return new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                RedirectUrl = GetRedirectUrl(type, data),
                DataJson = System.Text.Json.JsonSerializer.Serialize(data),
                Data = data // Store in memory only (not persisted directly)
            };
        }

    }
}