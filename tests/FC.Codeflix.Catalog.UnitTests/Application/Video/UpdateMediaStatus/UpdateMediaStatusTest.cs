using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateMediaStatus;

[Collection(nameof(UpdateMeidaStatusTestFixture))]
public class UpdateMediaStatusTest
{
    private readonly Mock<IVideoRepository> _videoRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly UseCase.UpdateMediaStatus _useCase;
    private readonly UpdateMeidaStatusTestFixture _fixture;

    public UpdateMediaStatusTest(UpdateMeidaStatusTestFixture fixture)
    {
        _fixture = fixture;
        _useCase = new UseCase.UpdateMediaStatus(
            _videoRepoMock.Object,
            _uowMock.Object,
            Mock.Of<ILogger<UseCase.UpdateMediaStatus>>()
        );
    }

    [Fact(DisplayName = nameof(UpdateStatusMediaWithSuccededEnconding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async void UpdateStatusMediaWithSuccededEnconding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetSuccededEncodingInput(exampleVideo.Id);

        _videoRepoMock.Setup(x => x.Get(
            exampleVideo.Id,
            It.IsAny<CancellationToken>())
        )
        .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Rating.Should().Be(exampleVideo.Rating);
        exampleVideo.Media!.Status.Should().Be(EMediaStatus.Completed);
        exampleVideo.Media!.EncodedPath.Should().Be(input.EncodedPath);

        _videoRepoMock.VerifyAll();
        _videoRepoMock.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()
        ), Times.Once);

        _uowMock.Verify(x => x.Commit(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(UpdateStatusMediaWhenFailEncoding))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async void UpdateStatusMediaWhenFailEncoding()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var input = _fixture.GetFailedEncodingInput(exampleVideo.Id);

        _videoRepoMock.Setup(x => x.Get(
            exampleVideo.Id,
            It.IsAny<CancellationToken>())
        )
        .ReturnsAsync(exampleVideo);

        VideoModelOutput output = await _useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleVideo.Id);
        output.Title.Should().Be(exampleVideo.Title);
        output.Description.Should().Be(exampleVideo.Description);
        output.Opened.Should().Be(exampleVideo.Opened);
        output.Duration.Should().Be(exampleVideo.Duration);
        output.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        output.Rating.Should().Be(exampleVideo.Rating);
        exampleVideo.Media!.Status.Should().Be(EMediaStatus.Error);
        exampleVideo.Media!.EncodedPath.Should().BeNull();

        _videoRepoMock.VerifyAll();
        _videoRepoMock.Verify(x => x.Update(
            exampleVideo, It.IsAny<CancellationToken>()
        ), Times.Once);

        _uowMock.Verify(x => x.Commit(
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact(DisplayName = nameof(HandleWhenInvalidInput))]
    [Trait("Application", "UpdateMediaStatus - Use Cases")]
    public async void HandleWhenInvalidInput()
    {
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        var expectedEncodedPath = exampleVideo.Media!.EncodedPath;
        var expectedStatus = exampleVideo.Media!.Status;
        string expectedErrorMessaging = "Invalid Media Status";
        var input = _fixture.GetInvalidStatusInput(exampleVideo.Id);

        _videoRepoMock.Setup(x => x.Get(
            exampleVideo.Id,
            It.IsAny<CancellationToken>())
        )
        .ReturnsAsync(exampleVideo);

        var action = async () => await _useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedErrorMessaging);

        exampleVideo.Media!.Status.Should().Be(expectedStatus);
        exampleVideo.Media!.EncodedPath.Should().Be(expectedEncodedPath);

        _videoRepoMock.VerifyAll();

        _videoRepoMock.Verify(x => x.Update(
           exampleVideo, It.IsAny<CancellationToken>()
       ), Times.Never);

        _uowMock.Verify(x => x.Commit(
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
