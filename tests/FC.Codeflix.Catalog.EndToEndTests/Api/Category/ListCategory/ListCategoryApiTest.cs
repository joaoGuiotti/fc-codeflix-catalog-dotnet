using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategory;

[Collection(nameof(ListCategoryApiTestFixtureColletion))]
public class ListCategoryApiTest : IDisposable
{
    private readonly ListCategoryApiTestFixture _fixture;

    public ListCategoryApiTest(ListCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategoryListAndTotals))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task GetCategoryListAndTotals()
    {
        // Arrange
        var defaultPerPage = 15;
        var categories = _fixture.GetExampleCategoryList(22).ToList();
        await _fixture.Persistence.BulkInsert(categories);

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Meta.Total.Should().Be(categories.Count);
        output.Data.Should().HaveCount(defaultPerPage);

        foreach (var item in output.Data)
        {
            var exampleItem = categories
                .Find(x => x.Id == item.Id)!;
            _fixture.CompareEntityOutput(exampleItem, item);
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        // Arrange
        var categories = _fixture.GetExampleCategoryList(20).ToList();
        await _fixture.Persistence.BulkInsert(categories);
        var input = new ListCategoriesInput(page: 1, perPage: 5);

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories", input);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(categories.Count);
        output.Data.Should().HaveCount(input.PerPage);

        foreach (var item in output.Data)
        {
            var exampleItem = categories
                .Find(x => x.Id == item.Id)!;
            _fixture.CompareEntityOutput(exampleItem, item);
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        // Arrange
        var categories = _fixture
            .GetExampleCategoryList(quantityCategoriesToGenerate)
            .ToList();
        await _fixture.Persistence.BulkInsert(categories);
        var input = new ListCategoriesInput(page, perPage);

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories", input);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(categories.Count);
        output.Data.Should().HaveCount(expectedQuantityItems);

        foreach (var item in output.Data)
        {
            var exampleItem = categories
                .Find(x => x.Id == item.Id)!;
            _fixture.CompareEntityOutput(exampleItem, item);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
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
        // Arrange
        var categoryNamesList = new List<string>() {
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
        var categories = _fixture
            .GetExampleCategoriesListWithNames(categoryNamesList);
        await _fixture.Persistence.BulkInsert(categories);
        var input = new ListCategoriesInput(page, perPage, search);

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories", input);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var item in output.Data)
        {
            var exampleItem = categories
                .Find(x => x.Id == item.Id)!;
            _fixture.CompareEntityOutput(exampleItem, item);
        }
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
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
        // Arrange
        var categoryNamesList = new List<string>() {
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
        var categories = _fixture.GetExampleCategoriesListWithNames(categoryNamesList);
        await _fixture.Persistence.BulkInsert(categories);
        var inputOrder = order == "asc" ? ESearchOrder.Asc : ESearchOrder.Desc;
        var input = new ListCategoriesInput(
            page: 1,
            perPage: 20,
            sort: orderBy,
            sortDir: inputOrder
        );

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CategoryModelOutput>>($"/categories", input);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(categories.Count);
        output.Data.Should().HaveCount(categories.Count);

        var expectedOrderedList = _fixture.CloneCategoriesListOrdered(
            categories,
            input.Sort ?? "",
            input.SortDir
        );

        for (int indice = 0; indice < expectedOrderedList.Count; indice++)
        {
            var outputItem = output.Data[indice];
            var exampleItem = expectedOrderedList[indice];
            _fixture.CompareEntityOutput(exampleItem, outputItem);
        }
    }

    void IDisposable.Dispose()
       => _fixture.DisposePersistence();
}

