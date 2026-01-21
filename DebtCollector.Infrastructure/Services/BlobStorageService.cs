using Azure.Storage.Blobs;
using DebtCollector.Application.Common.Interfaces;

namespace DebtCollector.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public BlobStorageService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string blobUrl)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobName = System.IO.Path.GetFileName(new Uri(blobUrl).AbsolutePath);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
