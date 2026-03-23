using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using JobConnect.Application.Abstractions;
using JobConnect.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace JobConnect.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("No file uploaded.");

        using var stream = file.OpenReadStream();
        var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (fileExt == ".jpg" || fileExt == ".jpeg" || fileExt == ".png")
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        var rawUploadParams = new RawUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = Path.GetFileNameWithoutExtension(file.FileName)
        };
        var rawUploadResult = await _cloudinary.UploadAsync(rawUploadParams);
        return rawUploadResult.SecureUrl.AbsoluteUri;
    }
}
