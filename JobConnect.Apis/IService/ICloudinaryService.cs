namespace JobConnect.Apis.IService
{
    public interface ICloudinaryService
    {
       // Task<string> UploadImageAsync(IFormFile file);
        Task<string> UploadAsync(IFormFile file);
    }
}
