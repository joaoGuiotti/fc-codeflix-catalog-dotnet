using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;


[Collection(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTest
{
    private readonly DeleteGenreTestFixture _fixture;

    public DeleteGenreTest(DeleteGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("Genre", "DeleteGenre - Use Cases")]
    public async void DeleteGenre()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genre = _fixture.GetExampleGenre();

        var useCase = new UseCase.DeleteGenre(repoMock.Object, uowMock.Object);

        var input = new UseCase.DeleteGenreInput(genre.Id);

        repoMock.Setup(
            x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(genre);

        await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
             x => x.Get(genre.Id, It.IsAny<CancellationToken>()),
             Times.Once
         );

        repoMock.Verify(
            x => x.Delete(genre, It.IsAny<CancellationToken>()),
            Times.Once
        );

        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowErrorWhenNotFound))]
    [Trait("Genre", "DeleteGenre - Use Cases")]
    public async void ThrowErrorWhenNotFound()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var genre = _fixture.GetExampleGenre();

        var input = new UseCase.DeleteGenreInput(genre.Id);

        repoMock
            .Setup(x => x.Get(genre.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new NotFoundException($"Category with ID {genre.Id} not found.")
            );

        var useCase = new UseCase.DeleteGenre(
            repoMock.Object,
            uowMock.Object
        );

        var task = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repoMock.Verify(
            x => x.Get(genre.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


}
