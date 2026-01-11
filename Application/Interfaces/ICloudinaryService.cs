using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(string publicId, string url)> UploadFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string publicId);
        string GetImageUrl(string publicId, string transformation = null);

        string GetPublicIdFromUrl(string imageUrl);
    }
}
