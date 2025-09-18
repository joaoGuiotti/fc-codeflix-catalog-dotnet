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
    public List<Category> GetCategories(int length = 10)
        => Enumerable.Range(1, length)
            .Select(_ => GetValidCategory())
            .ToList();

    public ListCategoriesInput GetValidInput()
        => new ListCategoriesInput(
            page: 1,
            perPage: 10,
            search: "",
            sort: "name",
            sortDir: ESearchOrder.ASC
        );
        
}
