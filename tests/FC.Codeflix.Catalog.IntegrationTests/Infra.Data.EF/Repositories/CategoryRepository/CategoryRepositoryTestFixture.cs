using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection
    : ICollectionFixture<CategoryRepositoryTestFixture>
{

}

public class CategoryRepositoryTestFixture
    : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetInvalidCategoryNameTooShort()
        => Faker.Lorem.Letter(2);

    // Description
    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 1000)
            description = description[..1000];
        return description;
    }

    public string GetInvalidCategoryDescriptionTooLong()
        => Faker.Lorem.Letter(1001);


    public bool GetRandomBoolean()
        => Faker.Random.Bool();


    public Category GetExampleCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var dbContext = new CodeflixCatalogDbContext(CreateDbContextOptions());
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    private DbContextOptions<CodeflixCatalogDbContext> CreateDbContextOptions()
        => new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase($"IntegrationTestsDb_{Guid.NewGuid()}")
            .Options;
}
