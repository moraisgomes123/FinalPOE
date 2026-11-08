using Azure.Storage.Blobs;using Azure.Storage.Blobs.Models;
namespace RaceDay.Api.Services;
public class AzureBlobStorageService:IBlobStorageService
{
 private readonly BlobServiceClient _client;
 public AzureBlobStorageService(IConfiguration configuration){var connection=configuration["AzureBlobStorage:ConnectionString"];if(string.IsNullOrWhiteSpace(connection))throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");_client=new BlobServiceClient(connection);}
 public async Task<string> UploadAsync(Stream content,string fileName,string contentType,string containerName,CancellationToken cancellationToken=default){var container=_client.GetBlobContainerClient(containerName);await container.CreateIfNotExistsAsync(PublicAccessType.None,cancellationToken:cancellationToken);var safeName=$"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";var blob=container.GetBlobClient(safeName);await blob.UploadAsync(content,new BlobHttpHeaders{ContentType=contentType},cancellationToken:cancellationToken);return blob.Uri.ToString();}
}
