using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.EndToEndTests.Common;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

public class CastMemberApiBaseFixture : BaseFixture
{
    public CastMemberApiPersistence Persistence;

    public CastMemberApiBaseFixture()
    {
        Persistence = new CastMemberApiPersistence(CreateDbContext());
    }

    public string GetValidName()
    {
        var name = "";
        while (name.Length < 3)
            name = Faker.Name.FullName();
        if (name.Length > 255)
            name = name[..255];
        return name;
    }

    public string GetInvalidNameTooShort()
        => Faker.Lorem.Letter(2);

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);

    public ECastMemberType GetRandomType()
        => Faker.Random.Enum<ECastMemberType>();

    public ECastMemberType GetDifferentType(ECastMemberType type)
        => type == ECastMemberType.Actor
            ? ECastMemberType.Director
            : ECastMemberType.Actor;

    public DomainEntity.CastMember GetExampleCastMember(
        string? name = null,
        ECastMemberType? type = null
    ) => DomainEntity.CastMember.Create(
        name ?? GetValidName(),
        type ?? GetRandomType()
    );

    public List<DomainEntity.CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable.Range(0, length)
            .Select(_ =>
            {
                Thread.Sleep(1);
                return GetExampleCastMember();
            })
            .ToList();

    public List<DomainEntity.CastMember> GetExampleCastMembersListByNames(List<string> names)
        => names.Select((name, index) =>
            {
                Thread.Sleep(1);
                return GetExampleCastMember(
                    name,
                    index % 2 == 0 ? ECastMemberType.Actor : ECastMemberType.Director
                );
            })
            .ToList();

    public void CompareEntityOutput(DomainEntity.CastMember entity, CastMemberOutput output)
    {
        entity.Should().NotBeNull();
        output.Should().NotBeNull();
        entity.Id.Should().Be(output.Id);
        entity.Name.Should().Be(output.Name);
        entity.Type.Should().Be(output.type);
        entity.CreatedAt.Should().BeSameDateAs(output.CreatedAt);
    }
}
