using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[Collection(nameof(CreateVideoTestFixture))]
public class CreateVideoTest
{
    private readonly CreateVideoTestFixture _fixture;

    public CreateVideoTest(CreateVideoTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateVideo))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideo()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateVideo(repoMock.Object, uowMock.Object);
        var input = _fixture.CreateVideInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(x => x.Insert(
            It.Is<DomainEntity.Video>(video =>
                video.Title == input.Title &&
                video.Published == input.Published &&
                video.Description == input.Description &&
                video.Duration == input.Duration &&
                video.Rating == input.Rating &&
                video.Id != Guid.Empty &&
                video.YearLaunched == input.YearLaunched &&
                video.Opened == input.Opened
            ),
            It.IsAny<CancellationToken>()
        ));
        uowMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBe(default(DateTime));
        output.Title.Should().Be(input.Title);
        output.Published.Should().Be(input.Published);
        output.Description.Should().Be(input.Description);
        output.Duration.Should().Be(input.Duration);
        output.Rating.Should().Be(input.Rating);
        output.YearLaunched.Should().Be(input.YearLaunched);
        output.Opened.Should().Be(input.Opened);
    }

    [Fact(DisplayName = nameof(CreateVideoThrowsWhenInvalidInput))]
    [Trait("Video", "Create Video - Use Cases")]
    public async Task CreateVideoThrowsWhenInvalidInput()
    {
        var repoMock = _fixture.GetRepoMock();
        var uowMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCase.CreateVideo(repoMock.Object, uowMock.Object);
        var input = _fixture.CreateVideInput("");

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        var expectedAssertion = await action.Should().ThrowAsync<EntityValidationException>();

        expectedAssertion.WithMessage("There are validation errors.")
            .Which.Errors.Should().Contain(x => x.PropertyName == "Title" && x.ErrorMessage == "Title is required");

        repoMock.Verify(x =>
                x.Insert(
                    It.IsAny<DomainEntity.Video>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }
}
