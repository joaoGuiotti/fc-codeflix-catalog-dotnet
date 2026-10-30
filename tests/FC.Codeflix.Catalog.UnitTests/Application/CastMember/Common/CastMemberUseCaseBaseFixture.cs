using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.Common;

public class CastMemberUseCaseBaseFixture : BaseFixture
{
    public Mock<ICastMemberRepository> GetRepositoryMock()
        => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock()
        => new();

    public string GetValidName()
        => Faker.Name.FullName();

    public string GetInvalidNameTooShort()
        => Faker.Lorem.Letter(2);

    public string GetInvalidNameTooLong()
        => Faker.Lorem.Letter(256);

    public ECastMemberType GetRandomType()
        => Faker.Random.Enum<ECastMemberType>();

    public DomainEntity.CastMember GetExampleCastMember(
        string? name = null,
        ECastMemberType? type = null
    ) => DomainEntity.CastMember.Create(
        name ?? GetValidName(),
        type ?? GetRandomType()
    );

    public List<DomainEntity.CastMember> GetExampleCastMembersList(int length = 10)
        => Enumerable
            .Range(1, length)
            .Select(_ => GetExampleCastMember())
            .ToList();
}
