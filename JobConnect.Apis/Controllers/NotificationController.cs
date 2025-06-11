using JobConnect.Apis.DTO_s;
using JobConnect.Apis.IService;
using JobConnect.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JobConnect.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requires JWT token
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token." });

            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(new { message = "Notifications retrieved successfully.", data = notifications });
        }

        [HttpPost("device-token")]
        public async Task<IActionResult> SubscribeDeviceToken([FromBody] SubscribeDeviceTokenRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token." });

            await _notificationService.SubscribeDeviceTokenAsync(userId, request.PushToken, request.Platform);
            return Ok(new { message = "Device token subscribed successfully." });
        }
    }


}