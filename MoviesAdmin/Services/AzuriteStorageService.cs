using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAdmin.Services
{
    public class AzuriteStorageService : IAzuriteStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName;

        public AzuriteStorageService(IConfiguration configuration)
        {
            _configuration = configuration;

            containerName = _configuration["AzureStorage:ContainerName"] ?? throw new InvalidOperationException("Missing configuration: AzureStorage:ContainerName");
            string connectionString = _configuration["AzureStorage:ConnectionString"] ?? throw new InvalidOperationException("Missing configuration: AzureStorage:ConnectionString");

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string filename)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(filename);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                }
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, options);
            }

            return blobClient.Uri.ToString();
        }

        // Requirement C: Deletes the previously uploaded cover photo from the storage service
        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return;
            }

            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // Parse out the filename accurately from the absolute storage URI
                var uri = new Uri(fileUrl);
                string filename = Path.GetFileName(uri.LocalPath);

                var blobClient = containerClient.GetBlobClient(filename);

                // Erase the file blob along with any snapshots it possesses
                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            }
            catch (Exception)
            {
                // Soft-catch failures to prevent application crashes if a file was manually scrubbed
            }
        }
    }
}
