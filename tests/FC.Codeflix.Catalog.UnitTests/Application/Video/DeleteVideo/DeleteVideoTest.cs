using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[Collection(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTest
{
    private readonly DeleteVideoTestFixture _fixture;
    private readonly Mock<IVideoRepository> _repoMock = new();
    private readonly Mock<IStorageService> _storageServiceMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly UseCase.DeleteVideo _useCase;

    public DeleteVideoTest(DeleteVideoTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UseCase.DeleteVideo(
            _repoMock.Object,
            _storageServiceMock.Object,
            _uowMock.Object
        );
    }

    [Fact(DisplayName = nameof(DeleteVideo))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideo()
    {
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithAllMediasAndClearStorage))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithAllMediasAndClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        List<string> filePaths = [videoExample.Media!.FilePath, videoExample.Trailer!.FilePath];
        var input = _fixture.GetValidInput(videoExample.Id);

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePaths.Contains(filePath)),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Exactly(2));
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyTrailerAndClearStorage))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyTrailerAndClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateTrailer(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePath == videoExample.Trailer!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithOnlyMediaAndClearStorage))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithOnlyMediaAndClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        videoExample.UpdateMedia(_fixture.GetValidMediaPath());
        var input = _fixture.GetValidInput(videoExample.Id);

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.Is<string>(filePath => filePath == videoExample.Media!.FilePath),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(DeleteVideoWithoutAnyMediaAndDontClearStorage))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoWithoutAnyMediaAndDontClearStorage()
    {
        var videoExample = _fixture.GetValidVideo();
        var input = _fixture.GetValidInput(videoExample.Id);

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == videoExample.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(videoExample);

        await _useCase.Handle(input, CancellationToken.None);

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.Is<DomainEntity.Video>(video => video.Id == videoExample.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact(DisplayName = nameof(DeleteVideoThrowsWhenVideoNotFound))]
    [Trait("Video", "DeleteVideo - Use Cases")]
    public async Task DeleteVideoThrowsWhenVideoNotFound()
    {
        var input = _fixture.GetValidInput();

        _repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == input.Id),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new NotFoundException($"Video '{input.Id}' not found."));

        var action = () => _useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{input.Id}' not found.");

        _repoMock.VerifyAll();
        _repoMock.Verify(x => x.Delete(
            It.IsAny<DomainEntity.Video>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
        _uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        _storageServiceMock.Verify(x => x.Delete(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
