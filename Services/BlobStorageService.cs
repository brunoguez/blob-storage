using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace blob_storage.Services
{
    public class BlobStorageService(IConfiguration configuration)
    {
        private readonly BlobServiceClient _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]);
        private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"];

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString(); // Retorna a URL do arquivo
        }

        public async Task<List<string>> ListFilesAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var files = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                files.Add(blobItem.Name);
            }

            return files;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<(Stream?, string?)> DownloadFileAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                return (null, null);
            }

            var downloadInfo = await blobClient.DownloadAsync();
            return (downloadInfo.Value.Content, downloadInfo.Value.ContentType);
        }
    }
}
