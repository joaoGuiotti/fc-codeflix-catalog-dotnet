using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[Collection(nameof(UploadMediasTestFixture))]
public class UploadMediasTest
{
    private readonly UploadMediasTestFixture _fixture;

    private readonly Mock<IVideoRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _uowRepoMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();

    private readonly UseCase.UploadMedias _useCase;

    public UploadMediasTest(UploadMediasTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UseCase.UploadMedias(
            _repoMock.Object,
            _storageServiceMock.Object,
            _uowRepoMock.Object
        );
    }

    [Fact(DisplayName = nameof(UploadMedias))]
    [Trait("Video", "UploadMedias - Use Cases")]
    public async Task UploadMedias()
    {
        var input = _fixture.GetValidInput();
        var exampleVideo = _fixture.GetValidVideo();

        _repoMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleVideo);

        _storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(Guid.NewGuid().ToString());

        await _useCase.Handle(input, CancellationToken.None);

        _storageServiceMock.Verify(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(2)
        );

        _uowRepoMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));
    }

    [Fact(DisplayName = nameof(UploadMediasThrowWhenNotFoundVideo))]
    [Trait("Video", "UploadMedias - Use Cases")]
    public async Task UploadMediasThrowWhenNotFoundVideo()
    {
        var input = _fixture.GetValidInput();

        _repoMock.Setup(x => x.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException($"Video '{input.VideoId}' not found."));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{input.VideoId}' not found.");
    }

    [Fact(DisplayName = nameof(ClearStorageInUploadErrorCase))]
    [Trait("Video", "UploadMedias - Use Cases")]
    public async Task ClearStorageInUploadErrorCase()
    {
        var video = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(video.Id);
        var videoFileName = StorageFileName.Create(video.Id, nameof(video.Media), input.VideoFile!.Extension);
        var trailerFileName = StorageFileName.Create(video.Id, nameof(video.Trailer), input.TrailerFile!.Extension);

        var videoStoragePath = $"storage/{videoFileName}";
        var trailerStoragePath = $"storage/{trailerFileName}";

        List<string> fileNames = [videoFileName, trailerFileName];

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(guid => guid == video.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(video);

        _storageServiceMock.Setup(x => x.Upload(
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoStoragePath);

        _storageServiceMock.Setup(x => x.Upload(
            It.Is<string>(fileName => fileName == trailerFileName),
            It.IsAny<Stream>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new Exception($"Something went wrong with the upload {trailerFileName}."));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<Exception>()
            .WithMessage($"Something went wrong with the upload {trailerFileName}.");

        _repoMock.VerifyAll();
        _storageServiceMock.Verify(x => x.Upload(
                It.Is<string>(fileName => fileNames.Contains(fileName)),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2)
        );
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(fileName => fileName == videoStoragePath),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(1));

        _uowRepoMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }
}
