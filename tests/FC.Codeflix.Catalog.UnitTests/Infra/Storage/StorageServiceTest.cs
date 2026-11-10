using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Storage.Configuration;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;
using InfraStorage = FC.Codeflix.Catalog.Infra.Storage.Service;
using GcpData = Google.Apis.Storage.v1.Data;

namespace FC.Codeflix.Catalog.UnitTests.Infra.Storage;

[Collection(nameof(StorageServiceTestFixture))]
public class StorageServiceTest
{
    private readonly StorageServiceTestFixture _fixture;

    private readonly Mock<StorageClient> _storageClientMock = new();

    private readonly StorageServiceOptions _storageOptions;

    private readonly IStorageService _service;

    public StorageServiceTest(StorageServiceTestFixture fixture)
    {
        _fixture = fixture;
        _storageOptions = new StorageServiceOptions { BucketName = _fixture.GetBucketName() };
        _service = new InfraStorage.StorageService(
            _storageClientMock.Object,
            Options.Create(_storageOptions)
        );
    }

    [Fact(DisplayName = nameof(Upload))]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Upload()
    {
        var objectMock = new Mock<GcpData.Object>();

        _storageClientMock.Setup(x => x.UploadObjectAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
        )).ReturnsAsync(objectMock.Object);


        var fileName = _fixture.GetFileName();
        var contentStream = Encoding.UTF8.GetBytes(_fixture.GetContentFile());
        var stream = new MemoryStream(contentStream);
        var contentType = _fixture.GetContentType();

        var filePath = await _service.Upload(fileName, stream, contentType, CancellationToken.None);

        Assert.Equal(fileName, filePath);

        _storageClientMock.Verify(x => x.UploadObjectAsync(
            _storageOptions.BucketName,
            fileName,
            contentType,
            stream,
            It.IsAny<UploadObjectOptions>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<IProgress<IUploadProgress>>()
        ), Times.Once);
    }


    [Fact(DisplayName = nameof(Delete))]
    [Trait("Infra.Storage", "StorageService")]
    public async Task Delete()
    {
        _storageClientMock.Setup(x => x.DeleteObjectAsync(
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()
        ));
        var fileName = _fixture.GetFileName();

        await _service.Delete(fileName, CancellationToken.None);

        _storageClientMock.Verify(x => x.DeleteObjectAsync(
            _storageOptions.BucketName, fileName,
            It.IsAny<DeleteObjectOptions>(), It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    //Usado somente para teste rapido de integração com o GCP, não deve ser usado em produção

    //[Fact(DisplayName = nameof(IntegrationTest))]
    //[Trait("Infra.Storage", "StorageService")]
    //public async Task IntegrationTest()
    //{
    //    Environment.SetEnvironmentVariable(
    //        "GOOGLE_APPLICATION_CREDENTIALS",
    //        "C:\\DEV\\github\\admin-catalogo-de-videos-c-shrap\\src\\FC.Codeflix.Catalog.Api\\gcp-credentials.json"
    //    );

    //    var client = StorageClient.Create();
    //    var options = new StorageServiceOptions { BucketName = "dotnet-medias-catalog-admin" };
    //    var storageService = new InfraStorage.StorageService(client, Options.Create(options));

    //    await storageService.Delete(
    //        "test/file.txt",
    //        CancellationToken.None
    //    );
    //}
}
