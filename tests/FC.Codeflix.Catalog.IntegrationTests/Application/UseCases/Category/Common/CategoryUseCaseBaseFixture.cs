using FC.Codeflix.Catalog.IntegrationTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

public class CategoryUseCaseBaseFixture : BaseFixture
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

    public string GetInvalidCategoryNameTooShort() => Faker.Lorem.Letter(2);

    // Description
    public string GetValidCategoryDescription()
    {
        var description = Faker.Commerce.ProductDescription();
        if (description.Length > 1000)
            description = description[..1000];
        return description;
    }

    public string GetInvalidCategoryDescriptionTooLong() =>
        Faker.Lorem.Letter(1001);

    public DomainEntity.Category GetExampleCategory(
        string? name = null,
        string? description = null,
        bool? isActive = null
    ) =>
        new(
            name ?? GetValidCategoryName(),
            description ?? GetValidCategoryDescription(),
            isActive ?? GetRandomBoolean()
        );

    public List<DomainEntity.Category> GetExampleCategoryList(
        int length = 10
    ) => Enumerable.Range(1, length).Select(_ => GetExampleCategory()).ToList();

    public List<DomainEntity.Category> GetCategoriesListWithNames(
        List<string> names
    ) => names.Select(name => GetExampleCategory(name)).ToList();
}
