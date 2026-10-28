
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreTestFixture))]
public class UpdateGenreTestFixtureCollection
    : ICollectionFixture<UpdateGenreTestFixture>
{ }

public class UpdateGenreTestFixture : GenreUseCaseBaseFixture
{
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
