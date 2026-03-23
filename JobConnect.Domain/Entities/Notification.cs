using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace JobConnect.Domain.Entities;

public enum NotificationType
{
    JobMatch,
    ApplicationStatus,
    Message,
    Recommendation
}

[Table("Notifications")]
public class Notification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    public string DataJson { get; set; } = string.Empty;

    [NotMapped]
    public object? Data
    {
        get => JsonSerializer.Deserialize<object>(DataJson ?? "{}");
        set => DataJson = value != null ? JsonSerializer.Serialize(value) : "{}";
    }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
