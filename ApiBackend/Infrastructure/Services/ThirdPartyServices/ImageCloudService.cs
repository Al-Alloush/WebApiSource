using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Core.Helpers.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services.ThirdPartyServices
{

    public class ImageCloudService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public ImageCloudService(IOptions<CloudinarySettings> config)
        {
            Account account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<AppImageUploadResult> UploadPhotoAsync(IFormFile file, ImageTransformation transform)
        {
            if (file.Length > 0)
            {
                // using to dispose of this stream, because it's going to consume memory as soon as we're finished with this method.
                await using var stream = file.OpenReadStream();

                ImageUploadParams uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    /* transform to squer Image*/
                    Transformation = AddTransformation(transform)
                };

                ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new Exception(uploadResult.Error.Message);

                return new AppImageUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };

            }
            return null;
        }


        private Transformation AddTransformation(ImageTransformation transform)
        {
            var transformation = new Transformation();
            if (transform.Height > 0)
                transformation.Height(transform.Height);
            if (transform.Width > 0)
                transformation.Width(transform.Width);
            if (transform.Crop != null)
                transformation.Crop(transform.Crop);
            if (transform.Radius != null)
                transformation.Radius(transform.Radius);

            return transformation;

        }

        /// <summary>
        /// Delete Photo from Cloudinary Server
        /// </summary>
        /// <param name="publicId"></param>
        /// <returns>true if success, else return false</returns>
        /// 
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? true : false;
        }
    }




    /// <summary>
    /// To access to the values of the CloudName the ApiKey and the ApiSecrets after add in app seting the Cloudinary Account Details then in side startup.cs: 
    /// services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"))
    /// </summary>
    /// 
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

}
