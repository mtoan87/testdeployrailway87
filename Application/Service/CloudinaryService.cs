using Application.Commons;
using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinaryConfig _config;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(
           IConfiguration configuration,
           ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            _config = configuration.GetSection("CloudinaryConfig").Get<CloudinaryConfig>();

            var account = new Account(
                _config.CloudName,
                _config.ApiKey,
                _config.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }
        public async Task<(string publicId, string url)> UploadFileAsync(IFormFile file, string folder)
        {
            {
                try
                {
                    // ✅ Thêm kiểm tra định dạng file tại đây
                    var allowedMimeTypes = new List<string>
        {
            "image/jpeg",
            "image/png",
            "image/heic", // thêm loại MIME cho .heic
            "image/heif"  // một số HEIC có MIME là heif
        };

                    if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
                    {
                        throw new NotSupportedException("Unsupported image format.");
                    }

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        Folder = $"{_config.Folder}/{folder}",
                        Transformation = new Transformation()
                        .Width(_config.Transformation.Width)
                        .Height(_config.Transformation.Height)
                        .Crop(_config.Transformation.Crop)
                        .Quality(_config.Transformation.Quality)
                        .FetchFormat("jpg") // bạn có thể set "jpg" ở đây nếu cần chuyển đổi định dạng
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    return (uploadResult.PublicId, uploadResult.SecureUrl.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file to Cloudinary");
                    throw;
                }
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from Cloudinary");
                return false;
            }
        }

        public string GetImageUrl(string publicId, string transformation = null)
        {
            try
            {
                var url = _cloudinary.Api.UrlImgUp
                    .Transform(new Transformation()
                        .Width(_config.Transformation.Width)
                        .Height(_config.Transformation.Height)
                        .Crop(_config.Transformation.Crop)
                        .Quality(_config.Transformation.Quality)
                        .FetchFormat(_config.Transformation.Format))
                    .BuildUrl(publicId);

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Cloudinary URL");
                throw;
            }
        }
        public string GetPublicIdFromUrl(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.Segments;
                var publicId = segments[segments.Length - 1].Split('.')[0];
                return publicId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting public ID from URL");
                throw;
            }
        }
    }
}
