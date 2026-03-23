using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions;

public interface INotificationService
{
    Task SendNotificationToUserAsync(string userId, Notification notification);
    Task MarkAsReadAsync(string notificationId);
    Task<List<Notification>> GetAllForUserAsync(string userId);
    Task RegisterExpoTokenAsync(string userId, string expoToken);
    Task SendPushNotificationAsync(string expoToken, string title, string body, NotificationType type, object? data = null);
    Task SendJobMatchNotification(string userId, string jobId, string jobTitle);
    Task SendApplicationStatusNotification(string userId, string applicationId, string status);
    Task SendMessageNotification(string userId, string messageId, string senderName);
    Task SendRecommendationNotification(string userId, string recommendationText);
}
