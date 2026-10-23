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
            ("name", ESearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", ESearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", ESearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", ESearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderByDescending(x => x.CreatedAt),
        };
        return orderedEnumerable.ToList();
    }
}
