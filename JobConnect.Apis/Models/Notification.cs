using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("Notifications")]
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = "message";
        public string Title { get; set; }
        public string Message { get; set; }
        public string Redirect { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } // Foreign key to User (JobSeeker)

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}