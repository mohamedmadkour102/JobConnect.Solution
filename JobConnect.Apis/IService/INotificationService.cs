using JobConnect.Core.Models;

namespace JobConnect.Apis.IService
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, Notification notification);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task SubscribeDeviceTokenAsync(string userId, string pushToken, string platform);
    }
}
