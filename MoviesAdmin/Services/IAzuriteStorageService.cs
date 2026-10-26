namespace MoviesAdmin.Services
{
    public interface IAzuriteStorageService
    {
        public Task<string> UploadFileAsync(IFormFile file, string fileName);
    }
}
