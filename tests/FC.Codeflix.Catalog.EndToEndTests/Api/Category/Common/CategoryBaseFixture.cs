
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.EndToEndTests.Common;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

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
    public string GetInvalidNameTooLong() => Faker.Lorem.Letter(256);
    public string GetInvalidNameTooShort() => Faker.Lorem.Letter(2);
    public string GetInvalidDescriptionTooLong() => Faker.Lorem.Letter(10_001);

    public DomainEntity.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    public IEnumerable<DomainEntity.Category> GetExampleCategoryList(int length = 22)
        => Enumerable.Range(1, length)
            .Select(_ => GetExampleCategory()).ToList();

    public void CompareEntityOutput(DomainEntity.Category a, CategoryModelOutput b)
    {
        a.Should().NotBeNull();
        b.Should().NotBeNull();
        a.Name.Should().Be(b.Name);
        a.Description.Should().Be(b.Description);
        a.IsActive.Should().Be(b.IsActive);
        a.CreatedAt.Should().Be(b.CreatedAt);
    }
}
