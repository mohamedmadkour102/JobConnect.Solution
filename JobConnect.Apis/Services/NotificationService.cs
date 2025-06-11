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
            // تعيين معرف المستخدم
            notification.UserId = userId;

            // إضافة الإشعار إلى قاعدة البيانات
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // جلب توكن Expo للمستخدم
            var deviceToken = await _context.DeviceTokens
                .FirstOrDefaultAsync(dt => dt.UserId == userId);

            if (deviceToken != null)
            {
                // إرسال الإشعار المباشر
                await SendPushNotificationAsync(
                    deviceToken.PushToken,
                    notification.Title,
                    notification.Message
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

        public async Task SendPushNotificationAsync(string expoToken, string title, string body)
        {
            var message = new
            {
                to = expoToken,
                sound = "default",
                title = title,
                body = body,
                data = new { }
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
    }
} 