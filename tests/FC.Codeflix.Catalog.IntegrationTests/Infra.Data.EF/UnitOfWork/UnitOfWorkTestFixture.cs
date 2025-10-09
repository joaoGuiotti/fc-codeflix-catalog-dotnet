using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

[CollectionDefinition(nameof(UnitOfWorkTestFixtureCollection))]
public class UnitOfWorkTestFixtureCollection
    : ICollectionFixture<UnitOfWorkTestFixture> { }

public class UnitOfWorkTestFixture : BaseFixture
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

    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 1000)
            description = description[..1000];
        return description;
    }

    public bool GetRandomBoolean() => Faker.Random.Bool();

    public Category GetExampleCategory(
        string? name = null,
        string? description = null,
        bool? isActive = null
    ) =>
        new(
            name ?? GetValidCategoryName(),
            description ?? GetValidCategoryDescription(),
            isActive ?? GetRandomBoolean()
        );

    public List<Category> GetExampleCategoryList(int length = 10) =>
        Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();

    public List<Category> GetCategoriesListWithNames(List<string> names) =>
        names.Select(name => GetExampleCategory(name)).ToList();
}
