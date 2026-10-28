using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[Collection(nameof(GetGenreTestFixture))]
public class GetGenreTest
{
    private readonly GetGenreTestFixture _fixture;
    public GetGenreTest(GetGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Genre", "GetGenre - UseCases")]
    public async Task GetGenre()
    {
        var context = _fixture.CreateDbContext();
        var repo = new GenreRepository(context);
        var categoryRepo = new CategoryRepository(context);

        var useCase = new UseCase.GetGenre(repo, categoryRepo);

        var genre = _fixture.GetExampleGenre();

        await context.Genres.AddAsync(genre);
        await context.SaveChangesAsync();

        var input = _fixture.GetValidInput(id: genre.Id);

        var result = await useCase.Handle(input, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(input.Id);
        result.Name.Should().Be(genre.Name);
        result.CreatedAt.Should().BeSameDateAs(genre.CreatedAt);
        result.IsActive.Should().Be(genre.IsActive);
    }

    [Fact(DisplayName = nameof(GetGenreWithCategories))]
    [Trait("Genre", "GetGenre - UseCases")]
    public async Task GetGenreWithCategories()
    {
        var context = _fixture.CreateDbContext();
        var repo = new GenreRepository(context);
        var categoryRepo = new CategoryRepository(context);

        var useCase = new UseCase.GetGenre(repo, categoryRepo);

        var genre = _fixture.GetExampleGenre();

        var categories = _fixture.GetExampleCategoryList(10);

        categories.ForEach(x => genre.AddCategory(x.Id));

        await context.Categories.AddRangeAsync(categories);
        await context.Genres.AddAsync(genre);

        await context.GenresCategories.AddRangeAsync(categories.Select(cat => new GenresCategories(cat.Id, genre.Id)));

        await context.SaveChangesAsync();

        var input = _fixture.GetValidInput(id: genre.Id);

        var result = await useCase.Handle(input, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(input.Id);
        result.Name.Should().Be(genre.Name);
        result.CreatedAt.Should().BeSameDateAs(genre.CreatedAt);
        result.IsActive.Should().Be(genre.IsActive);
        foreach (var category in result.Categories)
        {
            var expectedCategory = categories.Single(x => x.Id == category.Id);
            category.Name.Should().Be(expectedCategory.Name);
        }
    }


    [Fact(DisplayName = nameof(GetGenreThrowWhenNotFound))]
    [Trait("Genre", "GetGenre - UseCases")]
    public async Task GetGenreThrowWhenNotFound()
    {
        var context = _fixture.CreateDbContext();
        var repo = new GenreRepository(context);
        var categoryRepo = new CategoryRepository(context);

        var useCase = new UseCase.GetGenre(repo, categoryRepo);

        var input = _fixture.GetValidInput();

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Genre with ID {input.Id} not found.");
    }
}
