using Bogus;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.UnitTests.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection
    : ICollectionFixture<UpdateCategoryTestFixture>
{ }

public class UpdateCategoryTestFixture
    : CategoryBaseFixture
{ 
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
