using Core.Interfaces.Shared.Services;
using DTOs.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace Services.Implementation.Shared
{
    public class UploadFileService : IUploadFileService
    {
        public UploadFileService()
        {

        }

        public async Task<Response<bool>> RemoveFromCurrentDirectory(string Image)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), Image);
            File.Delete(filePath);
            return new Response<bool>(true, "File Deleted successfully");
        }

        public async Task<Response<string>> Uploading(IFormFile file, string fileSetting, string folder)
        {
            if (file == null || file.Length == 0)
            {
                return new Response<string>("No file uploaded.");
            }

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";
            string? folderName = fileSetting + folder;

            var pathToSave = $"BaseURL/{folderName}";

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return new Response<string>(dbPath, "File Uploaded.");
        }
    }
}
