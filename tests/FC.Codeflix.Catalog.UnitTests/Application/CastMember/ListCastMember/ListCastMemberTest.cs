using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;

namespace FC.Codeflix.Catalog.UnitTests.Application.CastMember.ListCastMember;

[Collection(nameof(ListCastMemberTestFixture))]
public class ListCastMemberTest
{
    private readonly ListCastMemberTestFixture _fixture;

    public ListCastMemberTest(ListCastMemberTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCastMember - Use Cases")]
    public async Task List()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var castMembers = _fixture.GetExampleCastMembersList();
        var input = _fixture.GetValidInput();
        repoMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SearchOutput<DomainEntity.CastMember>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    total: castMembers.Count,
                    items: castMembers
                )
            );
        var useCase = new UseCase.ListCastMember(repoMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(castMembers.Count);
        output.Items.Should().HaveCount(castMembers.Count);
        output.Items.ToList().ForEach(outputItem =>
        {
            var castMember = castMembers.Find(x => x.Id == outputItem.Id);
            castMember.Should().NotBeNull();
            outputItem.Name.Should().Be(castMember!.Name);
            outputItem.type.Should().Be(castMember.Type);
            outputItem.CreatedAt.Should().Be(castMember.CreatedAt);
        });

        repoMock.Verify(
            x => x.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page == input.Page
                    && searchInput.PerPage == input.PerPage
                    && searchInput.Search == input.Search
                    && searchInput.Sort == input.Sort
                    && searchInput.SortDir == input.SortDir
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereAreNoCastMembers))]
    [Trait("Application", "ListCastMember - Use Cases")]
    public async Task ListReturnsEmptyWhenThereAreNoCastMembers()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetValidInput();
        repoMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SearchOutput<DomainEntity.CastMember>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    total: 0,
                    items: new List<DomainEntity.CastMember>()
                )
            );
        var useCase = new UseCase.ListCastMember(repoMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().BeEmpty();
        repoMock.Verify(
            x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ListUsesDefaultsWhenInputHasNoParameters))]
    [Trait("Application", "ListCastMember - Use Cases")]
    public async Task ListUsesDefaultsWhenInputHasNoParameters()
    {
        var repoMock = _fixture.GetRepositoryMock();
        repoMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SearchOutput<DomainEntity.CastMember>(
                    currentPage: 1,
                    perPage: 15,
                    total: 0,
                    items: new List<DomainEntity.CastMember>()
                )
            );
        var useCase = new UseCase.ListCastMember(repoMock.Object);

        await useCase.Handle(new UseCase.ListCastMemberInput(), CancellationToken.None);

        repoMock.Verify(
            x => x.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page == 1
                    && searchInput.PerPage == 15
                    && searchInput.Search == ""
                    && searchInput.Sort == ""
                    && searchInput.SortDir == ESearchOrder.Asc
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(ListForwardsInputParametersToRepository))]
    [Trait("Application", "ListCastMember - Use Cases")]
    [InlineData(1, 10, "brad", "name", ESearchOrder.Asc)]
    [InlineData(2, 20, "", "type", ESearchOrder.Desc)]
    [InlineData(3, 5, "actor", "createdAt", ESearchOrder.Asc)]
    [InlineData(1, 100, "", "", ESearchOrder.Desc)]
    public async Task ListForwardsInputParametersToRepository(
        int page,
        int perPage,
        string search,
        string sort,
        ESearchOrder sortDir
    )
    {
        var repoMock = _fixture.GetRepositoryMock();
        repoMock
            .Setup(x => x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SearchOutput<DomainEntity.CastMember>(
                    currentPage: page,
                    perPage: perPage,
                    total: 0,
                    items: new List<DomainEntity.CastMember>()
                )
            );
        var useCase = new UseCase.ListCastMember(repoMock.Object);
        var input = new UseCase.ListCastMemberInput(page, perPage, search, sort, sortDir);

        await useCase.Handle(input, CancellationToken.None);

        repoMock.Verify(
            x => x.Search(
                It.Is<SearchInput>(searchInput =>
                    searchInput.Page == page
                    && searchInput.PerPage == perPage
                    && searchInput.Search == search
                    && searchInput.Sort == sort
                    && searchInput.SortDir == sortDir
                ),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
