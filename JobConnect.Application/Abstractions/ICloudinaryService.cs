using Microsoft.AspNetCore.Http;

namespace JobConnect.Application.Abstractions;

public interface ICloudinaryService
{
    Task<string> UploadAsync(IFormFile file);
}
