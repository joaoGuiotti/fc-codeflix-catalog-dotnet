using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenre;

[Collection(nameof(ListGenreApiTestFixture))]
public class ListGenreApiTest : IDisposable
{
    private readonly ListGenreApiTestFixture _fixture;

    public ListGenreApiTest(ListGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListGenres))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListGenres()
    {
        var defaultPerPage = 15;
        var exampleGenres = _fixture.GetExampleGenreList(20);
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(defaultPerPage);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListGenresAndTotal))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListGenresAndTotal()
    {
        var exampleGenres = _fixture.GetExampleGenreList(20);
        await _fixture.Persistence.BulkInsert(exampleGenres);
        var input = new ListGenreInput(page: 1, perPage: 5);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(input.PerPage);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Fact(DisplayName = nameof(ListGenresWithRelations))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    public async Task ListGenresWithRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreList(15);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );

        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(exampleGenres.Count);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
            var relatedCategories = exampleCategories
                .Where(x => exampleItem!.Categories.Contains(x.Id))
                .ToList();
            _fixture.CompareOutputGenreCategories(outputItem.Categories, relatedCategories);
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityGenresToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleGenres = _fixture.GetExampleGenreList(quantityGenresToGenerate);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );

        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var input = new ListGenreInput(page, perPage);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(expectedQuantityItems);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
            var relatedCategories = exampleCategories
                .Where(x => exampleItem!.Categories.Contains(x.Id))
                .ToList();
            _fixture.CompareOutputGenreCategories(outputItem.Categories, relatedCategories);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var genreNamesList = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        };
        var exampleGenres = _fixture.GetExampleGenresListWithNames(genreNamesList);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );

        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var input = new ListGenreInput(page, perPage, search);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleGenres.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
            var relatedCategories = exampleCategories
                .Where(x => exampleItem!.Categories.Contains(x.Id))
                .ToList();
            _fixture.CompareOutputGenreCategories(outputItem.Categories, relatedCategories);
        }
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task ListOrdered(
        string orderBy,
        string order
    )
    {
        var genreNamesList = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future"
        };
        var exampleGenres = _fixture.GetExampleGenresListWithNames(genreNamesList);
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );

        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var inputOrder = order == "asc" ? ESearchOrder.Asc : ESearchOrder.Desc;
        var input = new ListGenreInput(
            page: 1,
            perPage: 20,
            sort: orderBy,
            sortDir: inputOrder
        );

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleGenres.Count);
        output.Data.Should().HaveCount(exampleGenres.Count);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
            exampleGenres,
            input.Sort ?? "",
            input.SortDir
        );

        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputItem = output.Data[indice];
            var exampleItem = expectedOrderedList[indice];
            _fixture.CompareEntityOutput(exampleItem, outputItem);
            var relatedCategories = exampleCategories
                .Where(x => exampleItem.Categories.Contains(x.Id))
                .ToList();
            _fixture.CompareOutputGenreCategories(outputItem.Categories, relatedCategories);
        }
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}

