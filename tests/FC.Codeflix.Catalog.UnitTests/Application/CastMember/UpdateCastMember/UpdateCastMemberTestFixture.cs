using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[CollectionDefinition(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTestFixtureCollection
    : ICollectionFixture<UpdateCastMemberTestFixture>
{ }

public class UpdateCastMemberTestFixture : CastMemberUseCaseBaseFixture
{
    public UseCase.UpdateCastMemberInput GetValidInput(
        Guid? id = null,
        ECastMemberType? type = null
    ) => new(
        id ?? Guid.NewGuid(),
        GetValidName(),
        type ?? GetRandomType()
    );
}
