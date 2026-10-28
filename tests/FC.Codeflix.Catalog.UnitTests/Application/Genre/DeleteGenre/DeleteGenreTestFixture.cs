using FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.DeleteGenre;

[CollectionDefinition(nameof(DeleteGenreTestFixture))]
public class DeleteGenreTestFixtureCollection
    : ICollectionFixture<DeleteGenreTestFixture>
{ }

public class DeleteGenreTestFixture
    : GenreUseCaseBaseFixture
{ }
