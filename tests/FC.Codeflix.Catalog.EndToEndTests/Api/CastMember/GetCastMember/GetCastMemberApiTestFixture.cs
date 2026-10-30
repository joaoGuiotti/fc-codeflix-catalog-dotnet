using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[CollectionDefinition(nameof(GetCastMemberApiTestFixture))]
public class GetCastMemberApiTestFixtureCollection
    : ICollectionFixture<GetCastMemberApiTestFixture>
{ }

public class GetCastMemberApiTestFixture : CastMemberApiBaseFixture
{ }
