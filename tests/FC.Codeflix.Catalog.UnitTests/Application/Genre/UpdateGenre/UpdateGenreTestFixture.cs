
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection
    : ICollectionFixture<UpdateGenreTestFixture>
{ }

public class UpdateGenreTestFixture : GenreUseCaseBaseFixture
{
    public string GetValidName()
        => Faker.Commerce.Categories(1)[0];

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);

    public DomainEntity.Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null
    )
    {
        var genre = new DomainEntity.Genre(
            GetValidName(),
            isActive ?? GetRandomBoolean()
        );

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public List<Guid> GetRandomIdsList(int? count = null)
        => Enumerable
            .Range(1, count ?? (new Random()).Next(1, 10))
            .Select(_ => Guid.NewGuid())
            .ToList();

    public UpdateGenreInput GetValidInput(Guid? id = null)
        => new(
            id ?? Guid.NewGuid(),
            GetValidName(),
            GetRandomBoolean()
        );

    public UpdateGenreInput GetValidInputWithCategories(
        Guid? id = null,
        List<Guid>? categoriesIds = null
    )
        => new(
            id ?? Guid.NewGuid(),
            GetValidName(),
            GetRandomBoolean(),
            categoriesIds ?? GetRandomIdsList()
        );
}
