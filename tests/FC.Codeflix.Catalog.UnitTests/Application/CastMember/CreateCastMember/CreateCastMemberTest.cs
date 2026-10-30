using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberTestFixture))]
public class CreateCastMemberTest
{
    private readonly CreateCastMemberTestFixture _fixture;

    public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(CreateCastMember))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public async Task CreateCastMember(ECastMemberType type)
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput(type);
        var useCase = new UseCase.CreateCastMember(repoMock.Object, uowMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
            x => x.Insert(
                It.Is<DomainEntity.CastMember>(castMember =>
                    castMember.Name == input.Name
                    && castMember.Type == input.Type
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.type.Should().Be(input.Type);
        output.CreatedAt.Should().NotBe(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    [InlineData(null, "Name cannot be null or empty.")]
    [InlineData("", "Name cannot be null or empty.")]
    [InlineData("   ", "Name cannot be null or empty.")]
    [InlineData("ab", "Name must be at least 3 characters long.")]
    public async Task ThrowWhenNameIsInvalid(
        string? name,
        string expectedMessage
    )
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = new UseCase.CreateCastMemberInput(name!, _fixture.GetRandomType());
        var useCase = new UseCase.CreateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsTooLong))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowWhenNameIsTooLong()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = new UseCase.CreateCastMemberInput(
            _fixture.GetInvalidNameTooLong(),
            _fixture.GetRandomType()
        );
        var useCase = new UseCase.CreateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    [Fact(DisplayName = nameof(ThrowWhenTypeIsInvalid))]
    [Trait("Application", "CreateCastMember - Use Cases")]
    public async Task ThrowWhenTypeIsInvalid()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = new UseCase.CreateCastMemberInput(
            _fixture.GetValidName(),
            (ECastMemberType)99
        );
        var useCase = new UseCase.CreateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Type must be a valid CastMember type.");
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    private static void AssertNothingWasPersisted(
        Mock<FC.Codeflix.Catalog.Domain.Repository.ICastMemberRepository> repoMock,
        Mock<FC.Codeflix.Catalog.Application.Interfaces.IUnitOfWork> uowMock
    )
    {
        repoMock.Verify(
            x => x.Insert(
                It.IsAny<DomainEntity.CastMember>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
