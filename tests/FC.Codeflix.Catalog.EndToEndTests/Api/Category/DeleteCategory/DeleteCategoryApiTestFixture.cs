using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[CollectionDefinition(nameof(DeleteCategoryApiTestFixtureCollection))]
public class DeleteCategoryApiTestFixtureCollection
       : ICollectionFixture<DeleteCategoryApiTestFixture>
{
}

public class DeleteCategoryApiTestFixture
       : CategoryBaseFixture
{
}
