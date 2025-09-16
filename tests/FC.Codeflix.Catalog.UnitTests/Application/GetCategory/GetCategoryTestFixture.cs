using FC.Codeflix.Catalog.UnitTests.Application.Common;
namespace FC.Codeflix.Catalog.UnitTests.Application.GetCategory;

public class GetCategoryTestFixture : CategoryBaseFixture
{ }


[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureCollection
    : ICollectionFixture<GetCategoryTestFixture>
{ }