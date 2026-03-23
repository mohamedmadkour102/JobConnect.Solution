namespace JobConnect.Application.DTOs
{
    public class SubscribeDeviceTokenRequest
    {
        public string PushToken { get; set; }
        public string Platform { get; set; } // "android" or "ios"
    }
}
