using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace JobConnect.Core.Models
{
    public enum NotificationType
    {
        JobMatch,
        ApplicationStatus,
        Message,
        Recommendation,
        CompleteProfile
    }

    [Table("Notifications")]
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; }
        public string DataJson { get; set; }
        
        [NotMapped]
        public object Data 
        {
            get => JsonSerializer.Deserialize<object>(DataJson ?? "{}");
            set => DataJson = JsonSerializer.Serialize(value);
        }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}