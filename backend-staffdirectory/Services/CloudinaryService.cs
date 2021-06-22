using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace backend_staffdirectory.Services {
    public interface ICloudinaryService {
        ImageUploadResult UploadPhoto(IFormFile file);
    }
    public class CloudinaryService : ICloudinaryService {
        private readonly IConfiguration _config;

        public CloudinaryService() {

        }

        public CloudinaryService(IConfiguration config) {
            _config = config;
        }

        public ImageUploadResult UploadPhoto(IFormFile file) {

            Account account = new() {
                Cloud = _config["CloudinaryCloudName"],
                ApiKey = _config["CloudinaryApiKey"],
                ApiSecret = _config["CloudinaryApiSecret"]
            };

            Cloudinary cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams() {
                File = new FileDescription($@"TempMedia/{file.FileName}"),
                UploadPreset = "cq2yqglg",
            };

            ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);

            return uploadResult;
        }
    }
}
