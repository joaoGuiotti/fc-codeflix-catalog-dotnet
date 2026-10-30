using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture>
{ }

public class CastMemberTestFixture : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember(
        string? name = null,
        ECastMemberType? type = null
    ) => DomainEntity.CastMember.Create(
        name ?? GetValidName(),
        type ?? GetRandomCastMemberType()
    );

    public string GetValidName()
        => Faker.Name.FullName();

    // Random.Next(1, 2) sempre retornava 1 (Director); Actor nunca era gerado.
    public ECastMemberType GetRandomCastMemberType()
        => Faker.Random.Enum<ECastMemberType>();

    public ECastMemberType GetDifferentType(ECastMemberType type)
        => type == ECastMemberType.Actor
            ? ECastMemberType.Director
            : ECastMemberType.Actor;

    public string GetInvalidNameTooShort()
        => Faker.Lorem.Letter(2);

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);
}
