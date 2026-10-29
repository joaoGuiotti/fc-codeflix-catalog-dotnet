using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[CollectionDefinition(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTestFixtureCollection
    : ICollectionFixture<UpdateGenreApiTestFixture>
{ }

public class UpdateGenreApiTestFixture : GenreApiBaseFixture
{
    public UpdateGenreInput GetExampleInput(
        Guid? id = null,
        List<Guid>? categoriesIds = null
    )
        => new(
            id ?? Guid.NewGuid(),
            GetValidName(),
            GetRandomBoolean(),
            categoriesIds
        );
}

