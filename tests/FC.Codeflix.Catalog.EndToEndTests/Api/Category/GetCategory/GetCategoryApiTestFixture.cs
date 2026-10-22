
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;

[CollectionDefinition(nameof(GetCategoryApiTestFixtureColletion))]
public class GetCategoryApiTestFixtureColletion
    : ICollectionFixture<GetCategoryApiTestFixture>
{ }

public class GetCategoryApiTestFixture
    : CategoryBaseFixture
{ }
