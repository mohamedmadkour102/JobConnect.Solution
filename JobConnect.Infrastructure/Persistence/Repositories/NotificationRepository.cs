using JobConnect.Application.Abstractions.Persistence;
using JobConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobConnect.Infrastructure.Persistence.Repositories;

public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context) { }

    public async Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default) =>
        await Context.Notifications.AddAsync(notification, cancellationToken);

    public async Task<Notification?> FindNotificationByIdAsync(string notificationId, CancellationToken cancellationToken = default) =>
        await Context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId, cancellationToken);

    public async Task<List<Notification>> GetNotificationsForUserAsync(string userId, CancellationToken cancellationToken = default) =>
        await Context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<DeviceToken?> GetDeviceTokenByUserIdAsync(string userId, CancellationToken cancellationToken = default) =>
        await Context.DeviceTokens
            .FirstOrDefaultAsync(dt => dt.UserId == userId, cancellationToken);

    public async Task UpsertDeviceTokenAsync(DeviceToken token, CancellationToken cancellationToken = default)
    {
        var existing = await Context.DeviceTokens
            .FirstOrDefaultAsync(dt => dt.UserId == token.UserId, cancellationToken);
        if (existing != null)
            existing.PushToken = token.PushToken;
        else
            await Context.DeviceTokens.AddAsync(token, cancellationToken);
    }
}
