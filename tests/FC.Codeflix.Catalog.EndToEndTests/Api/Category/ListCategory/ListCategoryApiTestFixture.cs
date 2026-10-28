using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategory;

[CollectionDefinition(nameof(ListCategoryApiTestFixtureColletion))]
public class ListCategoryApiTestFixtureColletion : ICollectionFixture<ListCategoryApiTestFixture>
{
}

public class ListCategoryApiTestFixture : CategoryBaseFixture
{
    public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
        => names.Select(name =>
        {
            Thread.Sleep(1);
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();

    public List<DomainEntity.Category> CloneCategoriesListOrdered(
        List<DomainEntity.Category> categoriesList,
        string orderBy,
        ESearchOrder order
    )
    {
        var listClone = new List<DomainEntity.Category>(categoriesList);
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
