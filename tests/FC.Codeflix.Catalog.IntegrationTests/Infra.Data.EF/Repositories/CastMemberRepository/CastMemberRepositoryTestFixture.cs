using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.IntegrationTests.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[CollectionDefinition(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTestFixtureCollection
    : ICollectionFixture<CastMemberRepositoryTestFixture>
{ }

public class CastMemberRepositoryTestFixture : BaseFixture
{
    public string GetValidName()
        => Faker.Name.FullName();

    public ECastMemberType GetRandomType()
        => Faker.Random.Enum<ECastMemberType>();

    public ECastMemberType GetDifferentType(ECastMemberType type)
        => type == ECastMemberType.Actor
            ? ECastMemberType.Director
            : ECastMemberType.Actor;

    public CastMember GetExampleCastMember(
        string? name = null,
        ECastMemberType? type = null
    ) => CastMember.Create(
        name ?? GetValidName(),
        type ?? GetRandomType()
    );

    public List<CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable
            .Range(1, length)
            .Select(_ => GetExampleCastMember())
            .ToList();

    /// <summary>
    /// Cria um CastMember por nome, alternando o tipo (Actor nos índices pares,
    /// Director nos ímpares) para garantir que os dois tipos estejam presentes.
    /// </summary>
    public List<CastMember> GetExampleCastMembersListByNames(List<string> names)
        => names
            .Select((name, index) => GetExampleCastMember(
                name,
                index % 2 == 0 ? ECastMemberType.Actor : ECastMemberType.Director
            ))
            .ToList();
}
