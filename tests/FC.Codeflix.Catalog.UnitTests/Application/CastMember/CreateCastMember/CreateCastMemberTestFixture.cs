using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[CollectionDefinition(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTestFixtureCollection
    : ICollectionFixture<CreateCastMemberTestFixture>
{ }

public class CreateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
    public UseCase.CreateCastMemberInput GetValidInput(ECastMemberType? type = null)
        => new(GetValidName(), type ?? GetRandomType());
}
