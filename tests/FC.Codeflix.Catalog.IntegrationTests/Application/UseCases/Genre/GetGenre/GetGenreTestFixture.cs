using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.GetGenre;

[CollectionDefinition(nameof(GetGenreTestFixture))]
public class GetGenreTestCollection : ICollectionFixture<GetGenreTestFixture>
{ }

public class GetGenreTestFixture : GenreUseCaseBaseFixture
{
    public GetGenreInput GetValidInput(Guid? id = null)
        => new(id ?? Guid.NewGuid());

    public List<Domain.Entity.Category> GetExampleCategoryList(int length = 10) =>
        Enumerable.Range(1, length).Select(_ => new Domain.Entity.Category(GetValidName(), GetValidName())).ToList();
}
