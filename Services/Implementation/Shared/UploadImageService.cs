using Core.DTOs.Shared;
using Core.Interfaces.Shared.Services;
using DTOs.Shared.Responses;
using System.Text;

namespace MOCA.Services.Implementation.Shared
{
    public class UploadImageService : IUploadImageService
    {
        public UploadImageService()
        {

        }

        public async Task<Response<string>> UploadImage(ImageUpload image, string fileSetting, string pathType)
        {
            try
            {
                string? folderName = fileSetting + "//" + image.AlbumName;
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                byte[] imageBytes = Convert.FromBase64String(image.Image);

                string renameFile = DateTime.UtcNow.Ticks + "_" + pathType + ".png";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fullPath = Path.Combine(pathToSave, renameFile);
                var dbPath = Path.Combine(folderName, renameFile);

                await File.WriteAllBytesAsync(fullPath, imageBytes);
                return new Response<string>(dbPath, "Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return new Response<string>($"Error uploading image: {ex.Message}");
            }
        }

        public async Task<Response<bool>> Remove(string hostLink, string Image)
        {
            StringBuilder imagePath = new StringBuilder();
            imagePath.Append(hostLink);
            imagePath.Append(Image);
            File.Delete(imagePath.ToString());
            return new Response<bool>(true, "Images Deleted successfully");
        }

        public async Task<Response<bool>> RemoveFromCurrentDirectory(string Image)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), Image);
            File.Delete(imagePath);
            return new Response<bool>(true, "Images Deleted successfully");
        }
    }
}
