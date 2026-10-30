using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[CollectionDefinition(nameof(DeleteCastMemberApiTestFixture))]
public class DeleteCastMemberApiTestFixtureCollection
    : ICollectionFixture<DeleteCastMemberApiTestFixture>
{ }

public class DeleteCastMemberApiTestFixture : CastMemberApiBaseFixture
{ }
