using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MoviesAdmin.Services
{
    public interface IAzuriteStorageService
    {
        public Task<string> UploadFileAsync(IFormFile file, string fileName);
        public Task DeleteFileAsync(string fileUrl);
    }
}
