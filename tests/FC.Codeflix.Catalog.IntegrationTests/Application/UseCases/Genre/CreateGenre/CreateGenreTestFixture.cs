using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using Moq;
namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureColleciton
    : ICollectionFixture<CreateGenreTestFixture>
{
}

public class CreateGenreTestFixture
    : GenreUseCaseBaseFixture
{
    public string GetValidName()
        => Faker.Commerce.Categories(1)[0];

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(1001);

    public string GetInvalidNameTooShort() =>
        Faker.Lorem.Letter(1001);

    public CreateGenreInput GetValidInput()
        => new(GetValidName(), GetRandomBoolean());

    public CreateGenreInput GetInputWithCategories(List<Guid>? categoriesIds = null)
    {
        var ramdomLength = (new Random().Next(1, Faker.Random.Number(2, 50)));
        var _categoriesIds = Enumerable.Range(1, ramdomLength)
            .Select(_ => Guid.NewGuid())
            .ToList();

        return new (
               GetValidName(),
               GetRandomBoolean(),
               categoriesIds ?? _categoriesIds
        );
    }
}
