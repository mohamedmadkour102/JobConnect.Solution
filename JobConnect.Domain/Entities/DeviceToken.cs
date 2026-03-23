using System.ComponentModel.DataAnnotations.Schema;

namespace JobConnect.Domain.Entities;

[Table("DeviceTokens")]
public class DeviceToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string PushToken { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User? User { get; set; }
}
