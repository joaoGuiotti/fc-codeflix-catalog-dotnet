using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;

    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Genre", "GetGenre - Use Cases")]
    public async void GetGenre()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();

        var useCase = new UseCase.GetGenre(repoMock.Object);

        var exampleGenre = _fixture.GetExampleGenre();
        var input = _fixture.GetValidInput(exampleGenre.Id);

        repoMock.Setup(x => x.Get(
            It.Is<Guid>(id => id == exampleGenre.Id),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleGenre);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(input.Id);
        output.Name.Should().Be(exampleGenre.Name);
        output.IsActive.Should().Be(exampleGenre.IsActive);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Genre", "GetGenre - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();

        var useCase = new UseCase.GetGenre(repoMock.Object);
        var input = new UseCase.GetGenreInput(Guid.NewGuid());

        repoMock
            .Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainEntity.Genre?)null!);

        Func<Task> action = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Genre with ID {input.Id} not found.");
    }
}
