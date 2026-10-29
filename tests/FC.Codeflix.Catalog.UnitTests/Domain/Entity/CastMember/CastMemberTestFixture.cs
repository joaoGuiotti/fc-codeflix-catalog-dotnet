using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.UnitTests.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[CollectionDefinition(nameof(CastMemberTestFixture))]
public class CastMemberTestFixtureCollection : ICollectionFixture<CastMemberTestFixture>
{ }

public class CastMemberTestFixture : BaseFixture
{
    public DomainEntity.CastMember GetExampleCastMember()
        => DomainEntity.CastMember.Create(
            GetValidName(),
            GetRandomCastMemberType()
        );

    public string GetValidName()
        => Faker.Name.FullName();

    public ECastMemberType GetRandomCastMemberType()
        => (ECastMemberType)new Random().Next(1, 2);

    public string GetInvalidNameTooShort()
        => Faker.Lorem.Letter(2);

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);
}
