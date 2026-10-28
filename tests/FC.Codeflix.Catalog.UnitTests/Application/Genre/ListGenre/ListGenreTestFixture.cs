using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.UseCases.Genre.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.ListGenre;

[CollectionDefinition(nameof(ListGenreTestFixture))]
public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture>
{ }

public class ListGenreTestFixture : GenreUseCaseBaseFixture
{
    public ListGenreInput GetValidInput()
        => new(
            page: Faker.Random.Int(1, 10),
            perPage: Faker.Random.Int(15, 100),
            search: Faker.Lorem.Word(),
            sort: Faker.PickRandom(new List<string> { "name", "createdAt" }),
             sortDir: Faker.PickRandom(
                new List<ESearchOrder> { ESearchOrder.Asc, ESearchOrder.Desc }
            )
        );

    public List<DomainEntity.Genre> GetGenres(int length = 0)
        => Enumerable
        .Range(1, length > 0 ? length : new Random().Next(10, 100))
        .Select(_ => GetExampleGenre())
        .ToList();
}
