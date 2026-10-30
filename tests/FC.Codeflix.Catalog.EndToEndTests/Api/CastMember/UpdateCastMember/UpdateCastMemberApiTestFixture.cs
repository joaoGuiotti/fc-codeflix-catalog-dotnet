using FC.Codeflix.Catalog.Api.Contracts.CastMembers;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[CollectionDefinition(nameof(UpdateCastMemberApiTestFixture))]
public class UpdateCastMemberApiTestFixtureCollection
    : ICollectionFixture<UpdateCastMemberApiTestFixture>
{ }

public class UpdateCastMemberApiTestFixture : CastMemberApiBaseFixture
{
    public UpdateCastMemberRequest GetExampleRequest(
        string? name = null,
        ECastMemberType? type = null
    ) => new(
        name ?? GetValidName(),
        type ?? GetRandomType()
    );
}
