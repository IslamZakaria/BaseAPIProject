using DTOs.Shared.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Shared.Services
{
    public interface IUploadFileService
    {
        Task<Response<string>> Uploading(IFormFile file, string fileSetting, string folder);
        Task<Response<bool>> RemoveFromCurrentDirectory(string file);
    }
}
