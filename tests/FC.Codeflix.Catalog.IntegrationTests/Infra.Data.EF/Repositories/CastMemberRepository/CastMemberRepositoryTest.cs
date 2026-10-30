using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository;

[Collection(nameof(CastMemberRepositoryTestFixture))]
public class CastMemberRepositoryTest
{
    private static readonly List<string> ExampleNames =
    [
        "Brad Pitt",
        "Angelina Jolie",
        "Christopher Nolan",
        "Quentin Tarantino",
        "Brad Bird",
        "Meryl Streep",
    ];

    private readonly CastMemberRepositoryTestFixture _fixture;

    public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public async Task Insert(ECastMemberType type)
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember(type: type);
        var repository = new Repository.CastMemberRepository(dbContext);

        await repository.Insert(exampleCastMember, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbCastMember = await assertsDbContext.CastMembers.FindAsync(exampleCastMember.Id);

        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var exampleList = _fixture.GetExampleCastMembersList(15);
        await dbContext.CastMembers.AddAsync(exampleCastMember);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var dbCastMember = await repository.Get(exampleCastMember.Id, CancellationToken.None);

        dbCastMember.Should().NotBeNull();
        dbCastMember.Id.Should().Be(exampleCastMember.Id);
        dbCastMember.Name.Should().Be(exampleCastMember.Name);
        dbCastMember.Type.Should().Be(exampleCastMember.Type);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        await dbContext.CastMembers.AddRangeAsync(_fixture.GetExampleCastMembersList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(dbContext);
        var randomGuid = Guid.NewGuid();

        var action = async () => await repository.Get(randomGuid, CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"CastMember with ID {randomGuid} not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        var anotherCastMember = _fixture.GetExampleCastMember();
        await dbContext.CastMembers.AddAsync(exampleCastMember);
        await dbContext.CastMembers.AddAsync(anotherCastMember);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var repository = new Repository.CastMemberRepository(actDbContext);

        await repository.Delete(exampleCastMember, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var deleted = await assertsDbContext
            .CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleCastMember.Id);
        var untouched = await assertsDbContext
            .CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == anotherCastMember.Id);
        deleted.Should().BeNull();
        untouched.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleCastMember = _fixture.GetExampleCastMember();
        await dbContext.CastMembers.AddAsync(exampleCastMember);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var repository = new Repository.CastMemberRepository(actDbContext);
        var newName = _fixture.GetValidName();
        var newType = _fixture.GetDifferentType(exampleCastMember.Type);

        exampleCastMember.Update(newName, newType);
        await repository.Update(exampleCastMember, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbCastMember = await assertsDbContext
            .CastMembers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleCastMember.Id);
        dbCastMember.Should().NotBeNull();
        dbCastMember!.Name.Should().Be(newName);
        dbCastMember.Type.Should().Be(newType);
        dbCastMember.CreatedAt.Should().Be(exampleCastMember.CreatedAt);
    }

    [Fact(DisplayName = nameof(SearchReturnListAndTotal))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersList(15);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );
        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleList.Count);
        output.Items.Should().HaveCount(exampleList.Count);
        output.Items.Should().OnlyHaveUniqueItems();
        AssertItemsBelongTo(output.Items, exampleList);
    }

    [Fact(DisplayName = nameof(SearchReturnEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    public async Task SearchReturnEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var repository = new Repository.CastMemberRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().BeEmpty();
    }

    [Theory(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchWithPagination(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersList(quantityToGenerate);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );
        var searchInput = new SearchInput(page, perPage, "", "", ESearchOrder.Asc);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.Should().OnlyHaveUniqueItems();
        AssertItemsBelongTo(output.Items, exampleList);
    }

    [Theory(DisplayName = nameof(SearchByName))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData("Brad", ESearchOrder.Asc, 1, 5, 2, 2)]
    [InlineData("Brad", ESearchOrder.Desc, 2, 1, 1, 2)]
    [InlineData("Brad", ESearchOrder.Desc, 3, 1, 0, 2)]
    [InlineData("Pitt", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Jolie", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Nolan", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("NotFound", ESearchOrder.Asc, 1, 5, 0, 0)]
    public async Task SearchByName(
        string searchTerm,
        ESearchOrder sortOrder,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersListByNames(ExampleNames);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );
        var searchInput = new SearchInput(page, perPage, searchTerm, "", sortOrder);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.Should().OnlyHaveUniqueItems();
        AssertItemsBelongTo(output.Items, exampleList);
    }

    [Theory(DisplayName = nameof(SearchByType))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public async Task SearchByType(ECastMemberType type)
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersListByNames(ExampleNames);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );
        var expectedItems = exampleList.Where(x => x.Type == type).ToList();
        var searchInput = new SearchInput(1, 20, type.ToString(), "", ESearchOrder.Asc);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Total.Should().Be(expectedItems.Count);
        output.Items.Should().HaveCount(expectedItems.Count);
        output.Items.Should().OnlyContain(x => x.Type == type);
        output.Items.Select(x => x.Id).Should().BeEquivalentTo(expectedItems.Select(x => x.Id));
    }

    [Theory(DisplayName = nameof(SearchWithOrderByAndOrder))]
    [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
    [InlineData("name", ESearchOrder.Asc)]
    [InlineData("name", ESearchOrder.Desc)]
    [InlineData("type", ESearchOrder.Asc)]
    [InlineData("type", ESearchOrder.Desc)]
    [InlineData("createdAt", ESearchOrder.Asc)]
    [InlineData("createdAt", ESearchOrder.Desc)]
    [InlineData("", ESearchOrder.Asc)]
    [InlineData("", ESearchOrder.Desc)]
    public async Task SearchWithOrderByAndOrder(
        string orderBy,
        ESearchOrder searchOrder
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleList = _fixture.GetExampleCastMembersListByNames(ExampleNames);
        await dbContext.CastMembers.AddRangeAsync(exampleList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var repository = new Repository.CastMemberRepository(
            _fixture.CreateDbContext(preserveData: true)
        );
        var searchInput = new SearchInput(1, 10, "", orderBy, searchOrder);

        var output = await repository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Total.Should().Be(exampleList.Count);
        output.Items.Should().HaveCount(exampleList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        var expectedOrderedList = (orderBy.ToLower(), searchOrder) switch
        {
            ("name", ESearchOrder.Asc) => exampleList
                .OrderBy(x => x.Name).ThenBy(x => x.Id).ToList(),
            ("name", ESearchOrder.Desc) => exampleList
                .OrderByDescending(x => x.Name).ThenBy(x => x.Id).ToList(),
            ("type", ESearchOrder.Asc) => exampleList
                .OrderBy(x => x.Type).ThenBy(x => x.Id).ToList(),
            ("type", ESearchOrder.Desc) => exampleList
                .OrderByDescending(x => x.Type).ThenBy(x => x.Id).ToList(),
            ("createdat", ESearchOrder.Asc) => exampleList
                .OrderBy(x => x.CreatedAt).ThenBy(x => x.Id).ToList(),
            ("createdat", ESearchOrder.Desc) => exampleList
                .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).ToList(),
            (_, ESearchOrder.Asc) => exampleList
                .OrderBy(x => x.CreatedAt).ThenBy(x => x.Id).ToList(),
            _ => exampleList
                .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id).ToList(),
        };

        for (var i = 0; i < output.Items.Count; i++)
        {
            output.Items[i].Id.Should().Be(expectedOrderedList[i].Id);
            output.Items[i].Name.Should().Be(expectedOrderedList[i].Name);
            output.Items[i].Type.Should().Be(expectedOrderedList[i].Type);
            output.Items[i].CreatedAt.Should().Be(expectedOrderedList[i].CreatedAt);
        }
    }

    private static void AssertItemsBelongTo(
        IReadOnlyList<CastMember> outputItems,
        List<CastMember> exampleList
    )
    {
        foreach (var outputItem in outputItems)
        {
            var exampleItem = exampleList.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Type.Should().Be(exampleItem.Type);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
}
