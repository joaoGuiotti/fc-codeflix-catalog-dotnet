using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FluentAssertions;
namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest
{
    private readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListGenreDefaultRequest))]
    [Trait("Genre", "ListGenre - UseCases")]
    public async void ListGenreDefaultRequest()
    {
        var repoMock = _fixture.GetGenreRepositoryMock();
        var categoryMock = _fixture.GetCategoryrepositoryMock();
        var genres = _fixture.GetGenres();
        var input = _fixture.GetValidInput();

        repoMock.Setup(x =>
            x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()
        )).ReturnsAsync(
            new SearchOutput<DomainEntity.Genre>(currentPage: input.Page,
                    perPage: input.PerPage,
                    total: genres.Count,
                    items: (IReadOnlyList<DomainEntity.Genre>)genres
                )
            );

        var useCase = new UseCase.ListGenre(repoMock.Object, categoryMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(genres.Count);
        output.Total.Should().Be(genres.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var genre = genres.Find(x => x.Id == outputItem.Id);
                genre.Should().NotBeNull();
                outputItem.Name.Should().Be(genre!.Name);
                outputItem.IsActive.Should().Be(genre.IsActive);
                outputItem.CreatedAt.Should().Be(genre.CreatedAt);
            });

        repoMock.Verify(
            x =>
                x.Search(
                    It.Is<SearchInput>(s =>
                        s.Page == input.Page
                        && s.PerPage == input.PerPage
                        && s.Search == input.Search
                        && s.Sort == input.Sort
                        && s.SortDir == input.SortDir
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
