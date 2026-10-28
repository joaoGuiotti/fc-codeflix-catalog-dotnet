using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestFixtureCollection
    : ICollectionFixture<GetGenreTestFixture>
{ }

public class GetGenreTestFixture
    : GenreUseCaseBaseFixture
{
    public GetGenreInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid());
}
