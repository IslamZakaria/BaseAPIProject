using Core.DTOs.Shared;
using DTOs.Shared.Responses;

namespace Core.Interfaces.Shared.Services
{
    public interface IUploadImageService
    {
        Task<Response<string>> UploadImage(ImageUpload image, string fileSetting, string pathType);
        Task<Response<bool>> RemoveFromCurrentDirectory(string Image);
    }
}
