using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberTestFixture))]
public class GetCastMemberTest
{
    private readonly GetCastMemberTestFixture _fixture;

    public GetCastMemberTest(GetCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task GetCastMember()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var input = new UseCase.GetCastMemberInput(exampleCastMember.Id);
        repoMock
            .Setup(x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exampleCastMember);
        var useCase = new UseCase.GetCastMember(repoMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Id.Should().Be(exampleCastMember.Id);
        output.Name.Should().Be(exampleCastMember.Name);
        output.type.Should().Be(exampleCastMember.Type);
        output.CreatedAt.Should().Be(exampleCastMember.CreatedAt);

        repoMock.Verify(
            x => x.Get(exampleCastMember.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenNotFound))]
    [Trait("Application", "GetCastMember - Use Cases")]
    public async Task ThrowWhenNotFound()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var input = new UseCase.GetCastMemberInput(Guid.NewGuid());
        repoMock
            .Setup(x => x.Get(input.Id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException($"CastMember with ID {input.Id} not found."));
        var useCase = new UseCase.GetCastMember(repoMock.Object);

        var action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember with ID {input.Id} not found.");
        repoMock.Verify(
            x => x.Get(input.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
