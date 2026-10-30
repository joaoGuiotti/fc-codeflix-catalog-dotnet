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
        castMember.Id.Should().NotBeEmpty();
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

    [Fact(DisplayName = nameof(InstantiateThrowErrorWhenTypeIsInvalid))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void InstantiateThrowErrorWhenTypeIsInvalid()
    {
        var instantiateAction = ()
            => DomainEntity.CastMember.Create("CastMember", (ECastMemberType)99);
        instantiateAction.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Type must be a valid CastMember type.");
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void Update()
    {
        var castMember = _fixture.GetExampleCastMember();
        var originalId = castMember.Id;
        var originalCreatedAt = castMember.CreatedAt;
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetDifferentType(castMember.Type);

        castMember.Update(newName, newType);

        castMember.Id.Should().Be(originalId);
        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(newType);
        castMember.CreatedAt.Should().Be(originalCreatedAt);
    }

    [Theory(DisplayName = nameof(UpdateKeepsCurrentTypeWhenTypeIsNull))]
    [Trait("Domain", "CastMember - Aggregate")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public void UpdateKeepsCurrentTypeWhenTypeIsNull(ECastMemberType currentType)
    {
        var castMember = _fixture.GetExampleCastMember(type: currentType);
        var newName = _fixture.GetValidName();

        castMember.Update(newName, null);

        castMember.Name.Should().Be(newName);
        castMember.Type.Should().Be(currentType);
    }

    [Theory(DisplayName = nameof(UpdateThrowError))]
    [Trait("Domain", "CastMember - Aggregate")]
    [MemberData(
        nameof(CastMemberTestDataGenerator.GetInvalidNames),
        MemberType = typeof(CastMemberTestDataGenerator)
    )]
    public void UpdateThrowError(
        string? name,
        string expectedErrorMessage
    )
    {
        var castMember = _fixture.GetExampleCastMember();

        var updateAction = () => castMember.Update(name!, castMember.Type);

        updateAction.Should()
            .Throw<EntityValidationException>()
            .WithMessage(expectedErrorMessage);
    }

    [Fact(DisplayName = nameof(UpdateThrowErrorWhenTypeIsInvalid))]
    [Trait("Domain", "CastMember - Aggregate")]
    public void UpdateThrowErrorWhenTypeIsInvalid()
    {
        var castMember = _fixture.GetExampleCastMember();

        var updateAction = ()
            => castMember.Update(_fixture.GetValidName(), (ECastMemberType)99);

        updateAction.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Type must be a valid CastMember type.");
    }
}
