using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[CollectionDefinition(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTestFixtureCollection
    : ICollectionFixture<DeleteCastMemberTestFixture>
{ }

public class DeleteCastMemberTestFixture : CastMemberUseCaseBaseFixture
{ }
