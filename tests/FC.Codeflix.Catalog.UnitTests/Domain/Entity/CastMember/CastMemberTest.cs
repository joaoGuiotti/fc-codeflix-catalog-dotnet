using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.CastMember;

[Collection(nameof(CastMemberTestFixture))]
public class CastMemberTest
{
    private readonly CastMemberTestFixture _fixture;

    public CastMemberTest(CastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void Instantiate()
    {
        DomainEntity.CastMember castMember = DomainEntity.CastMember.Create("CastMember", ECastMemberType.Actor);

        castMember.Should().NotBeNull();
        castMember.Name.Should().Be("CastMember");
        castMember.Type.Should().Be(ECastMemberType.Actor);
        castMember.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(InstantiateAnActor))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void InstantiateAnActor()
    {
        DomainEntity.CastMember castMember = DomainEntity.CastMember.CreateAnActor("CastMember");

        castMember.Should().NotBeNull();
        castMember.Name.Should().Be("CastMember");
        castMember.Type.Should().Be(ECastMemberType.Actor);
        castMember.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(InstantiateAnDirector))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void InstantiateAnDirector()
    {
        DomainEntity.CastMember castMember = DomainEntity.CastMember.CreateAnDirector("CastMember");

        castMember.Should().NotBeNull();
        castMember.Name.Should().Be("CastMember");
        castMember.Type.Should().Be(ECastMemberType.Director);
        castMember.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(InstantiateThrowError))]
    [Trait("Domain", "CastMember - Aggregate")]
    [MemberData(
        nameof(CastMemberTestDataGenerator.GetInvalidNames),
        MemberType = typeof(CastMemberTestDataGenerator)
    )]
    public void InstantiateThrowError(
        string? name,
        string expectedErrorMessage
    )
    {
        var instantiateAction = ()
            => DomainEntity.CastMember.CreateAnDirector(name!);
        instantiateAction.Should()
            .Throw<EntityValidationException>()
            .WithMessage(expectedErrorMessage);
    }
}

