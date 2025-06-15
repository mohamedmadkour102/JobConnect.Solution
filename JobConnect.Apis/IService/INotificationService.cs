using JobConnect.Core.Models;

namespace JobConnect.Core.IService
{
    public interface INotificationService
    {
        /// <summary>
        /// إرسال إشعار لمستخدم معين
        /// </summary>
        Task SendNotificationToUserAsync(string userId, Notification notification);

        /// <summary>
        /// تحديث حالة الإشعار كمقروء
        /// </summary>
        Task MarkAsReadAsync(string notificationId);

        /// <summary>
        /// جلب جميع إشعارات المستخدم
        /// </summary>
        Task<List<Notification>> GetAllForUserAsync(string userId);

        /// <summary>
        /// تسجيل توكن Expo للمستخدم
        /// </summary>
        Task RegisterExpoTokenAsync(string userId, string expoToken);

        /// <summary>
        /// إرسال إشعار مباشر عبر Expo
        /// </summary>
        Task SendPushNotificationAsync(string expoToken, string title, string body, NotificationType type, object data = null);

        // Task SendJobMatchNotification(string userId, string jobId, string jobTitle);
        Task SendCompleteProfileReminderAsync(string userId);
        Task SendApplicationStatusNotification(string userId, int applicationId, string status);
        Task SendMessageNotification(string userId, string messageId, string senderName);
        Task SendRecommendationNotification(string userId, string recommendationText);
    }
}