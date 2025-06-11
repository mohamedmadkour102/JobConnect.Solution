using FirebaseAdmin.Messaging;
using JobConnect.Apis.IService;
using JobConnect.Core.Models;
using JobConnect.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Notification = JobConnect.Core.Models.Notification;

namespace JobConnect.Apis.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _dbContext;

        public NotificationService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendNotificationAsync(string userId, Notification notification)
        {
            // Save notification to database
            notification.UserId = userId;
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            // Get user device tokens
            var deviceTokens = await _dbContext.DeviceTokens
                .Where(dt => dt.UserId == userId)
                .Select(dt => dt.PushToken)
                .ToListAsync();

            if (!deviceTokens.Any())
                return;

            // Send push notification via FCM
            var message = new MulticastMessage
            {
                Tokens = deviceTokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = notification.Title,
                    Body = notification.Message
                },
                Data = new Dictionary<string, string>
                {
                    { "redirect", notification.Redirect }
                }
            };

            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task SubscribeDeviceTokenAsync(string userId, string pushToken, string platform)
        {
            var existingToken = await _dbContext.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.UserId == userId && dt.Platform == platform);

            if (existingToken != null)
            {
                existingToken.PushToken = pushToken;
            }
            else
            {
                _dbContext.DeviceTokens.Add(new DeviceToken
                {
                    UserId = userId,
                    PushToken = pushToken,
                    Platform = platform
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}