using System.Text;
using System.Text.Json;
using JobConnect.Application.Abstractions;
using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;

namespace JobConnect.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpClientFactory _httpClientFactory;

    public NotificationService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendNotificationToUserAsync(string userId, Notification notification)
    {
        notification.UserId = userId;
        await _unitOfWork.Notifications.AddNotificationAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        var deviceToken = await _unitOfWork.Notifications.GetDeviceTokenByUserIdAsync(userId);
        if (deviceToken != null)
        {
            await SendPushNotificationAsync(
                deviceToken.PushToken,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Data);
        }
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var notification = await _unitOfWork.Notifications.FindNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<List<Notification>> GetAllForUserAsync(string userId) =>
        await _unitOfWork.Notifications.GetNotificationsForUserAsync(userId);

    public async Task RegisterExpoTokenAsync(string userId, string expoToken)
    {
        await _unitOfWork.Notifications.UpsertDeviceTokenAsync(new DeviceToken
        {
            UserId = userId,
            PushToken = expoToken,
            Platform = "android"
        });
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendPushNotificationAsync(string expoToken, string title, string body, NotificationType type, object? data = null)
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
            "application/json");

        var client = _httpClientFactory.CreateClient();
        await client.PostAsync("https://exp.host/--/api/v2/push/send", content);
    }

    private string GetRedirectUrl(NotificationType type, object? data)
    {
        return type switch
        {
            NotificationType.JobMatch => $"jobs/{((dynamic)data!).JobId}",
            NotificationType.ApplicationStatus => $"applications/{((dynamic)data!).ApplicationId}",
            NotificationType.Message => $"messages/{((dynamic)data!).MessageId}",
            NotificationType.Recommendation => "recommendations",
            _ => ""
        };
    }

    public async Task SendJobMatchNotification(string userId, string jobId, string jobTitle)
    {
        await SendNotificationToUserAsync(userId, new Notification
        {
            Type = NotificationType.JobMatch,
            Title = "New Job Match",
            Message = $"You've been matched with job: {jobTitle}",
            Data = new { JobId = jobId }
        });
    }

    public async Task SendApplicationStatusNotification(string userId, string applicationId, string status)
    {
        await SendNotificationToUserAsync(userId, new Notification
        {
            Type = NotificationType.ApplicationStatus,
            Title = "Application Update",
            Message = $"Your application status has changed to: {status}",
            Data = new { ApplicationId = applicationId }
        });
    }

    public async Task SendMessageNotification(string userId, string messageId, string senderName)
    {
        await SendNotificationToUserAsync(userId, new Notification
        {
            Type = NotificationType.Message,
            Title = $"New message from {senderName}",
            Message = "You have a new message",
            Data = new { MessageId = messageId }
        });
    }

    public async Task SendRecommendationNotification(string userId, string recommendationText)
    {
        await SendNotificationToUserAsync(userId, new Notification
        {
            Type = NotificationType.Recommendation,
            Title = "New Recommendation",
            Message = recommendationText,
            Data = new { }
        });
    }
}
