using Bogus;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection
    : ICollectionFixture<UpdateCategoryTestFixture>
{ }

public class UpdateCategoryTestFixture
    : BaseFixture
{

    public Mock<ICategoryRepository> GetRepositoryMock()
        => new();

    public Mock<IUnitOfWork> GetUowMock()
        => new();

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

    public bool GetRandomBoolean()
        => new Faker().Random.Bool();

    public Category GetValidCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetInvalidInputShortName()
        => new(
            Guid.NewGuid(),
            Faker.Lorem.Letter(2),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetInvalidInputTooLongName()
        => new(
            Guid.NewGuid(),
            new string('a',  256),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetInvalidInputTooLongDescription()
        => new(
            Guid.NewGuid(),
            GetValidCategoryName(),
            new string('a', 10_001),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetValidInput(Guid? id = null)
        => new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
}
