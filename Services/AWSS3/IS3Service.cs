using Microsoft.AspNetCore.Http;

namespace amazon_backend.Services.AWSS3
{
    public interface IS3Service
    {
        Task<List<string>> UploadFilesFromRange(List<IFormFile> files,string filePath);
        Task<string> UploadFile(IFormFile file,string filePath);
        Task<bool> DeleteFile(string filePath);
    }
}
