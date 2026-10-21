using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategory;

[Collection(nameof(ListCategoryTestFixtureCollection))]
public class ListCategoryTest
{
    private readonly ListCategoryTestFixture _fixture;

    public ListCategoryTest(ListCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategory - Use Cases")]
    public async Task List()
    {
        var dbContext = _fixture.CreateDbContext();
        var categories = _fixture.GetExampleCategoryList();
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        var repo = new CategoryRepository(dbContext);
        var useCase = new UseCase.ListCategories(repo);

        var input = new ListCategoriesInput(1, 10, "", "");
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(categories.Count);
        output.Total.Should().Be(categories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var category = categories.Find(x => x.Id == outputItem.Id);
                category.Should().NotBeNull();
                outputItem.Name.Should().Be(category!.Name);
                outputItem.Description.Should().Be(category.Description);
                outputItem.IsActive.Should().Be(category.IsActive);
                outputItem.CreatedAt.Should().Be(category.CreatedAt);
            });
    }

    [Fact(DisplayName = nameof(ListReturnsEmptyWhenPersistenceIsEmpty))]
    [Trait("Application", "ListCategory - Use Cases")]
    public async Task ListReturnsEmptyWhenPersistenceIsEmpty()
    {
        var dbContext = _fixture.CreateDbContext();
        var repo = new CategoryRepository(dbContext);
        var useCase = new UseCase.ListCategories(repo);

        var input = new ListCategoriesInput(1, 10, "", "");
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(0);
        output.Total.Should().Be(0);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
    }

    [Theory(DisplayName = nameof(SearchReturnsPaginatedResults))]
    [Trait("Application", "ListCategory - Use Cases")]
    [InlineData(10, 1, 10, 10)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginatedResults(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantity
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var categories = _fixture.GetExampleCategoryList(quantityToGenerate);
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        var repo = new CategoryRepository(dbContext);
        var useCase = new UseCase.ListCategories(repo);

        var input = new ListCategoriesInput(page, perPage);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(expectedQuantity);
        output.Total.Should().Be(categories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var category = categories.Find(x => x.Id == outputItem.Id);
                category.Should().NotBeNull();
                outputItem.Name.Should().Be(category!.Name);
                outputItem.Description.Should().Be(category.Description);
                outputItem.IsActive.Should().Be(category.IsActive);
                outputItem.CreatedAt.Should().Be(category.CreatedAt);
            });
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var categories = _fixture.GetExampleCategoryListWithNames(
            categoryNamesList
        );
        await dbContext.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var input = new ListCategoriesInput(page, perPage, search);
        var useCase = new UseCase.ListCategories(
            categoryRepository
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);

        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var category = categories.Find(x => x.Id == outputItem.Id);
                category.Should().NotBeNull();
                outputItem.Name.Should().Be(category!.Name);
                outputItem.Description.Should().Be(category.Description);
                outputItem.IsActive.Should().Be(category.IsActive);
                outputItem.CreatedAt.Should().Be(category.CreatedAt);
            });
    }
}
