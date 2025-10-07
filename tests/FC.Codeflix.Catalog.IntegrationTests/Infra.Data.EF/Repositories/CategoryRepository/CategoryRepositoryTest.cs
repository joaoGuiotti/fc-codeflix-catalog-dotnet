using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture _fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        await categoryRepository.Insert(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories.FindAsync(exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddAsync(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var dbCategory = await categoryRepository.Get(exampleCategory.Id, CancellationToken.None);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(exampleCategory.Id);
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var randomGuid = Guid.NewGuid();
        var task = async () => await categoryRepository.Get(randomGuid, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{randomGuid}' not found.");
    }


    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddAsync(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new Repository.CategoryRepository(dbContext);

        exampleCategory.Update(_fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());

        await categoryRepository.Update(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleCategory.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(exampleCategory.Name);
        dbCategory.Description.Should().Be(exampleCategory.Description);
        dbCategory.IsActive.Should().Be(exampleCategory.IsActive);
        dbCategory.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddAsync(exampleCategory);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Delete(exampleCategory, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbCategory = await _fixture.CreateDbContext().Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleCategory.Id);

        dbCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(SearchReturnListAndTotal))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var searchInput = new SearchInput(1, 30, "", "", ESearchOrder.Asc);
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleCategoryList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        });
    }

    [Fact(DisplayName = nameof(SearchReturnEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchReturnEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var searchInput = new SearchInput(1, 30, "", "", ESearchOrder.Asc);
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchWithPagination()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetExampleCategoryList(25);
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var searchInput = new SearchInput(2, 10, "", "name", ESearchOrder.Asc);
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(searchInput.PerPage);
        output.Items.Should().OnlyHaveUniqueItems();

        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleCategoryList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        });

        var expectedOrderedList = exampleCategoryList.OrderBy(x => x.Name).Skip(10).Take(10).ToList();
        for (int i = 0; i < output.Items.Count; i++)
        {
            (output.Items.ToList()[i].Id == expectedOrderedList[i].Id).Should().BeTrue();
        }
    }

    [Theory(DisplayName = nameof(SearchWithSearchFilter))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("Action", ESearchOrder.Asc, 1, 5, 3, 3)]
    [InlineData("Horror", ESearchOrder.Desc, 1, 5, 2, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 2, 1, 1, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 3, 1, 0, 2)]
    [InlineData("Drama", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Comedy", ESearchOrder.Asc, 1, 3, 2, 2)]
    [InlineData("Sci-fi", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Romance", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("NotFound", ESearchOrder.Asc, 1, 5, 0, 0)]
    public async Task SearchWithSearchFilter(
        string searchTerm,
        ESearchOrder sortOrder,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetCategoriesListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Action",
            "Drama",
            "Action Comedy",
            "Comedy",
            "Sci-fi",
            "Romance",
        });
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var searchInput = new SearchInput(page, perPage, searchTerm, "", sortOrder);
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.Should().OnlyHaveUniqueItems();

        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleCategoryList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        });
    }

    [Fact(DisplayName = nameof(SearchWithOrderByAndOrder))]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task SearchWithOrderByAndOrder()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCategoryList = _fixture.GetCategoriesListWithNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Action",
            "Drama",
            "Action Comedy",
            "Comedy",
        });
        await dbContext.AddRangeAsync(exampleCategoryList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        var searchInput = new SearchInput(1, 10, "", "name", ESearchOrder.Desc);
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleCategoryList.Count);
        output.Items.Should().HaveCount(exampleCategoryList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        output.Items.ToList().ForEach(outputItem =>
        {
            var exampleItem = exampleCategoryList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        });

        var expectedOrderedList = exampleCategoryList.OrderByDescending(x => x.Name).ToList();
        for (int i = 0; i < output.Items.Count; i++)
        {
            (output.Items.ToList()[i].Id == expectedOrderedList[i].Id).Should().BeTrue();
        }
    }
}
