
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixtureCollection))]
public class UpdateCategoryTestFixtureCollection
    : ICollectionFixture<UpdateCategoryTestFixture>
{ }

public class UpdateCategoryTestFixture
        : CategoryUseCaseBaseFixture
{
    public UpdateCategoryInput GetInvalidInputShortName() =>
       new(
           Guid.NewGuid(),
           Faker.Lorem.Letter(2),
           GetValidCategoryDescription(),
           GetRandomBoolean()
       );

    public UpdateCategoryInput GetInvalidInputTooLongName() =>
        new(
            Guid.NewGuid(),
            new string('a', 256),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetInvalidInputTooLongDescription() =>
        new(
            Guid.NewGuid(),
            GetValidCategoryName(),
            new string('a', 10_001),
            GetRandomBoolean()
        );

    public UpdateCategoryInput GetValidInput(Guid? id = null) =>
        new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );

}
