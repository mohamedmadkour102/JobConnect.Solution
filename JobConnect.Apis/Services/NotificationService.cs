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
            return type switch
            {
                NotificationType.JobMatch => $"jobs/{((dynamic)data).JobId}",
                NotificationType.ApplicationStatus => $"applications/{((dynamic)data).ApplicationId}",
                NotificationType.Message => $"messages/{((dynamic)data).MessageId}",
                NotificationType.Recommendation => "recommendations",
                _ => ""
            };
        }

        public async Task SendJobMatchNotification(string userId, string jobId, string jobTitle)
        {
            var notification = new Notification
            {
                Type = NotificationType.JobMatch,
                Title = "New Job Match",
                Message = $"You've been matched with job: {jobTitle}",
                Data = new { JobId = jobId }
            };
            
            await SendNotificationToUserAsync(userId, notification);
        }

        public async Task SendApplicationStatusNotification(string userId, string applicationId, string status)
        {
            var notification = new Notification
            {
                Type = NotificationType.ApplicationStatus,
                Title = "Application Update",
                Message = $"Your application status has changed to: {status}",
                Data = new { ApplicationId = applicationId }
            };
            
            await SendNotificationToUserAsync(userId, notification);
        }

        public async Task SendMessageNotification(string userId, string messageId, string senderName)
        {
            var notification = new Notification
            {
                Type = NotificationType.Message,
                Title = $"New message from {senderName}",
                Message = "You have a new message",
                Data = new { MessageId = messageId }
            };
            
            await SendNotificationToUserAsync(userId, notification);
        }

        public async Task SendRecommendationNotification(string userId, string recommendationText)
        {
            var notification = new Notification
            {
                Type = NotificationType.Recommendation,
                Title = "New Recommendation",
                Message = recommendationText,
                Data = new { }
            };
            
            await SendNotificationToUserAsync(userId, notification);
        }
    }
}