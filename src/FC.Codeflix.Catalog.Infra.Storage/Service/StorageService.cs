using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace FC.Codeflix.Catalog.Infra.Storage.Service;

public class StorageService : IStorageService
{
    private readonly StorageClient _storageClient;
    private readonly StorageServiceOptions _options;

    public StorageService(
        StorageClient storageClient,
        IOptions<StorageServiceOptions> options
    )
    {
        _storageClient = storageClient;
        _options = options.Value;
    }


    public async Task Delete(string filePath, CancellationToken ct)
        => await _storageClient.DeleteObjectAsync(
            _options.BucketName,
            filePath,
            cancellationToken: ct
        );

    public async Task<string> Upload(string fileName, Stream fileStream, string contentType, CancellationToken ct)
    {
        var result = await _storageClient.UploadObjectAsync(
            _options.BucketName,
            fileName,
            contentType,
            fileStream,
            cancellationToken: ct
        );

        return fileName;
    }
}
