using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberTestFixture))]
public class UpdateCastMemberTest
{
    private readonly UpdateCastMemberTestFixture _fixture;

    public UpdateCastMemberTest(UpdateCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCastMember))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    [InlineData(ECastMemberType.Actor, ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Actor, ECastMemberType.Director)]
    [InlineData(ECastMemberType.Director, ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director, ECastMemberType.Director)]
    public async Task UpdateCastMember(
        ECastMemberType currentType,
        ECastMemberType newType
    )
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember(type: currentType);
        var originalCreatedAt = exampleCastMember.CreatedAt;
        var input = _fixture.GetValidInput(exampleCastMember.Id, newType);
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(input.Name);
        output.type.Should().Be(newType);
        output.CreatedAt.Should().Be(originalCreatedAt);

        repoMock.Verify(
            x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.CastMember>(castMember =>
                    castMember.Id == exampleCastMember.Id
                    && castMember.Name == input.Name
                    && castMember.Type == newType
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(UpdateCastMemberWithoutProvidingType))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public async Task UpdateCastMemberWithoutProvidingType(ECastMemberType currentType)
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember(type: currentType);
        var input = new UseCase.UpdateCastMemberInput(
            exampleCastMember.Id,
            _fixture.GetValidName(),
            null
        );
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Name.Should().Be(input.Name);
        output.type.Should().Be(currentType);
        repoMock.Verify(
            x => x.Update(
                It.Is<DomainEntity.CastMember>(castMember =>
                    castMember.Id == exampleCastMember.Id
                    && castMember.Name == input.Name
                    && castMember.Type == currentType
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();
        repoMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember with ID {input.Id} not found."));
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember with ID {input.Id} not found.");
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
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
        var exampleCastMember = _fixture.GetExampleCastMember();
        var input = new UseCase.UpdateCastMemberInput(
            exampleCastMember.Id,
            name!,
            _fixture.GetRandomType()
        );
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsTooLong))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenNameIsTooLong()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var input = new UseCase.UpdateCastMemberInput(
            exampleCastMember.Id,
            _fixture.GetInvalidNameTooLong(),
            _fixture.GetRandomType()
        );
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage("Name must be less or equal 255 characters.");
        AssertNothingWasPersisted(repoMock, uowMock);
    }

    [Fact(DisplayName = nameof(ThrowWhenTypeIsInvalid))]
    [Trait("Application", "UpdateCastMember - Use Cases")]
    public async Task ThrowWhenTypeIsInvalid()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var input = _fixture.GetValidInput(exampleCastMember.Id, (ECastMemberType)99);
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.UpdateCastMember(repoMock.Object, uowMock.Object);

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
            x => x.Update(
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
