using JobConnect.Domain.Entities;

namespace JobConnect.Application.Abstractions.Persistence;

public interface INotificationRepository : IRepository<Notification>
{
    Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification?> FindNotificationByIdAsync(string notificationId, CancellationToken cancellationToken = default);
    Task<List<Notification>> GetNotificationsForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<DeviceToken?> GetDeviceTokenByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpsertDeviceTokenAsync(DeviceToken token, CancellationToken cancellationToken = default);
}
