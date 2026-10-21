using System;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategory;

[CollectionDefinition(nameof(ListCategoryTestFixtureCollection))]
public class ListCategoryTestFixtureCollection
    : ICollectionFixture<ListCategoryTestFixture>
{
}

public class ListCategoryTestFixture
    : CategoryUseCaseBaseFixture
{
    public List<Domain.Entity.Category> GetExampleCategoryListWithNames(List<string> names)
    {
        var categories = new List<Domain.Entity.Category>();

        foreach (var name in names)
        {
            var category = GetExampleCategory();
            category.Update(name);
            categories.Add(category);
        }

        return categories.ToList();
    }
}
