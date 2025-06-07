using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using JobConnect.Apis.Helpers;
using JobConnect.Apis.IService;
using Microsoft.Extensions.Options;

namespace JobConnect.Apis.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }


        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) throw new Exception("No file uploaded.");

            using var stream = file.OpenReadStream();
            var fileExt = Path.GetExtension(file.FileName).ToLower();

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
            else
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                   

                    PublicId = Path.GetFileNameWithoutExtension(file.FileName)
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
        }

    }
}



        //public async Task<string> UploadImageAsync(IFormFile file)
        //{
        //    if (file == null || file.Length == 0) throw new Exception("No file uploaded.");

        //    using var stream = file.OpenReadStream();
        //    var uploadParams = new ImageUploadParams
        //    {
        //        File = new FileDescription(file.FileName, stream),
        //        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        //    };
        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        //    return uploadResult.SecureUrl.AbsoluteUri;
        //}