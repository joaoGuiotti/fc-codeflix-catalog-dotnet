using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.ListCastMember;

[Collection(nameof(ListCastMemberApiTestFixture))]
public class ListCastMemberApiTest : IDisposable
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

    private readonly ListCastMemberApiTestFixture _fixture;

    public ListCastMemberApiTest(ListCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ListCastMembers))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    public async Task ListCastMembers()
    {
        var defaultPerPage = 15;
        var exampleCastMembers = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Meta.Total.Should().Be(exampleCastMembers.Count);
        output.Data.Should().HaveCount(defaultPerPage);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleCastMembers.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(15);
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListCastMembersAndTotal))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    public async Task ListCastMembersAndTotal()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(20);
        await _fixture.Persistence.BulkInsert(exampleCastMembers);
        var input = new ListCastMemberInput(1, 5, "", "", ESearchOrder.Asc);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleCastMembers.Count);
        output.Data.Should().HaveCount(input.PerPage);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleCastMembers.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(quantityToGenerate);
        await _fixture.Persistence.BulkInsert(exampleCastMembers);
        var input = new ListCastMemberInput(page, perPage, "", "", ESearchOrder.Asc);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleCastMembers.Count);
        output.Data.Should().HaveCount(expectedQuantityItems);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleCastMembers.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    [InlineData("Brad", 1, 5, 2, 2)]
    [InlineData("Brad", 2, 1, 1, 2)]
    [InlineData("Brad", 3, 1, 0, 2)]
    [InlineData("Pitt", 1, 5, 1, 1)]
    [InlineData("Jolie", 1, 5, 1, 1)]
    [InlineData("Nolan", 1, 5, 1, 1)]
    [InlineData("NotFound", 1, 5, 0, 0)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersListByNames(ExampleNames);
        await _fixture.Persistence.BulkInsert(exampleCastMembers);
        var input = new ListCastMemberInput(page, perPage, search, "", ESearchOrder.Asc);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data.Should().HaveCount(expectedQuantityItemsReturned);

        foreach (var outputItem in output.Data)
        {
            var exampleItem = exampleCastMembers.Find(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            _fixture.CompareEntityOutput(exampleItem!, outputItem);
        }
    }

    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "CastMember/ListCastMembers - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("type", "asc")]
    [InlineData("type", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    [InlineData("", "desc")]
    public async Task ListOrdered(
        string orderBy,
        string order
    )
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersListByNames(ExampleNames);
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var inputOrder = order == "asc" ? ESearchOrder.Asc : ESearchOrder.Desc;
        var input = new ListCastMemberInput(
            page: 1,
            perPage: 20,
            search: "",
            sort: orderBy,
            dir: inputOrder
        );

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponseList<CastMemberOutput>>("/castmember", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleCastMembers.Count);
        output.Data.Should().HaveCount(exampleCastMembers.Count);

        var expectedOrderedList = _fixture.CloneCastMembersListOrdered(
            exampleCastMembers,
            input.Sort ?? "",
            input.SortDir
        );

        for (var index = 0; index < expectedOrderedList.Count; index++)
        {
            var outputItem = output.Data[index];
            var exampleItem = expectedOrderedList[index];
            _fixture.CompareEntityOutput(exampleItem, outputItem);
        }
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
