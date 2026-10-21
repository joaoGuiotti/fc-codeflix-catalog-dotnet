
using FC.Codeflix.Catalog.EndToEndTests.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

public class CategoryBaseFixture
    : BaseFixture
{

    public CategoryPersistence Persistence;

    public CategoryBaseFixture()
        : base()
    {
        Persistence = new CategoryPersistence(CreateDbContext());
    }

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

    public bool GetRandomBoolean() => Faker.Random.Bool();
}
