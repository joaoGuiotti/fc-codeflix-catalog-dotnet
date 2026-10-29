using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreApiTestFixture))]
public class ListGenreApiTestFixtureCollection
    : ICollectionFixture<ListGenreApiTestFixture>
{ }

public class ListGenreApiTestFixture : GenreApiBaseFixture
{
    public List<Domain.Entity.Genre> GetExampleGenresListWithNames(List<string> names)
        => names.Select(name =>
        {
            Thread.Sleep(1);
            return GetExampleGenre(name: name);
        }).ToList();

    public List<Domain.Entity.Genre> CloneGenresListOrdered(
        List<Domain.Entity.Genre> genresList,
        string orderBy,
        ESearchOrder order
    )
    {
        var listClone = new List<Domain.Entity.Genre>(genresList);
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            ("name", ESearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            ("name", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ThenBy(x => x.Id),
            ("id", ESearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", ESearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
            ("createdat", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id),
            (_, ESearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
            _ => listClone.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id),
        };
        return orderedEnumerable.ToList();
    }
}

