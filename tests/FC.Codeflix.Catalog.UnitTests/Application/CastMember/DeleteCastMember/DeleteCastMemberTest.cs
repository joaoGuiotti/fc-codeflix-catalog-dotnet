using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberTestFixture))]
public class DeleteCastMemberTest
{
    private readonly DeleteCastMemberTestFixture _fixture;

    public DeleteCastMemberTest(DeleteCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task DeleteCastMember()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var input = new UseCase.DeleteCastMemberInput(exampleCastMember.Id);
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.DeleteCastMember(repoMock.Object, uowMock.Object);

        await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
            x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repoMock.Verify(
            x => x.Delete(exampleCastMember, It.IsAny<CancellationToken>()),
            Times.Once
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "DeleteCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var uowMock = _fixture.GetUnitOfWorkMock();
        var input = new UseCase.DeleteCastMemberInput(Guid.NewGuid());
        repoMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember with ID {input.Id} not found."));
        var useCase = new UseCase.DeleteCastMember(repoMock.Object, uowMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember with ID {input.Id} not found.");
        repoMock.Verify(
            x => x.Delete(It.IsAny<DomainEntity.CastMember>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        uowMock.Verify(
            x => x.Commit(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
