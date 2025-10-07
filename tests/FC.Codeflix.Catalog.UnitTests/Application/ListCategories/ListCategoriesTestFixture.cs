using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoryTestFixtureCollection
    : ICollectionFixture<ListCategoriesTestFixture>
{ }

public class ListCategoriesTestFixture
    : CategoryBaseFixture
{
    public List<Category> GetCategories(int length = 0)
        => Enumerable.Range(1, length > 0 ? length : new Random().Next(10, 100))
            .Select(_ => GetExampleCategory())
            .ToList();

    public ListCategoriesInput GetValidInput()
        => new ListCategoriesInput(
            page: Faker.Random.Int(1, 10),
            perPage: Faker.Random.Int(15, 100),
            search: Faker.Lorem.Word(),
            sort: Faker.PickRandom(new List<string> { "name", "createdAt" }),
            sortDir: Faker.PickRandom(new List<ESearchOrder> { ESearchOrder.Asc, ESearchOrder.Desc })
        );

}
