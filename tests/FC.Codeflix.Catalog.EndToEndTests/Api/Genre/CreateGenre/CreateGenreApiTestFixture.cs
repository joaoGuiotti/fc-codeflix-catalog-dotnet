using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTestFixtureCollection
    : ICollectionFixture<CreateGenreApiTestFixture>
{ }

public class CreateGenreApiTestFixture : GenreApiBaseFixture
{
    public CreateGenreInput GetExampleInput(List<Guid>? categoriesIds = null)
        => new(
            GetValidName(),
            GetRandomBoolean(),
            categoriesIds
        );
}

