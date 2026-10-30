using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberApiTestFixture))]
public class CreateCastMemberApiTestFixtureCollection
    : ICollectionFixture<CreateCastMemberApiTestFixture>
{ }

public class CreateCastMemberApiTestFixture : CastMemberApiBaseFixture
{
    public CreateCastMemberInput GetExampleInput(ECastMemberType? type = null)
        => new(
            GetValidName(),
            type ?? GetRandomType()
        );
}
