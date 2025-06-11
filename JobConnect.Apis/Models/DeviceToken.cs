using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Core.Models
{
    [Table("DeviceTokens")]
    public class DeviceToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string PushToken { get; set; }
        public string Platform { get; set; } // "android" or "ios"

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}