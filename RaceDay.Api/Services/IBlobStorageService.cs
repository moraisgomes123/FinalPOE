namespace RaceDay.Api.Services;

public interface IBlobStorageService
{
    Task<string> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        string containerName,
        CancellationToken cancellationToken = default
    );
}
