namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = Catalog.Domain.Entity;

public class CategoryTestFixture : BaseFixture
{
    public CategoryTestFixture()
     : base() { }

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

    public DomainEntity.Category GetValidCategory()
            => new(
                GetValidCategoryName(),
                GetValidCategoryDescription()
            );
}

[CollectionDefinition(nameof(CategoryTestFixture))]
public class CategoryTestFixtureCollection
    : ICollectionFixture<CategoryTestFixture>
{ }
