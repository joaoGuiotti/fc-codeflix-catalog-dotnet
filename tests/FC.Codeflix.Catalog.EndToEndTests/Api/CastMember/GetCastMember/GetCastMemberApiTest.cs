using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.GetCastMember;

[Collection(nameof(GetCastMemberApiTestFixture))]
public class GetCastMemberApiTest : IDisposable
{
    private readonly GetCastMemberApiTestFixture _fixture;

    public GetCastMemberApiTest(GetCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCastMember))]
    [Trait("EndToEnd/API", "CastMember/GetById - Endpoints")]
    public async Task GetCastMember()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var targetCastMember = exampleCastMembers[5];
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<CastMemberOutput>>($"/castmember/{targetCastMember.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        _fixture.CompareEntityOutput(targetCastMember, output.Data);
    }

    [Fact(DisplayName = nameof(GetCastMemberReturnStatus404NotFound))]
    [Trait("EndToEnd/API", "CastMember/GetById - Endpoints")]
    public async Task GetCastMemberReturnStatus404NotFound()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Get<ProblemDetails>($"/castmember/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"CastMember with ID {randomGuid} not found.");
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
