using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.DeleteCastMember;

[Collection(nameof(DeleteCastMemberApiTestFixture))]
public class DeleteCastMemberApiTest : IDisposable
{
    private readonly DeleteCastMemberApiTestFixture _fixture;

    public DeleteCastMemberApiTest(DeleteCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCastMember))]
    [Trait("EndToEnd/API", "CastMember/DeleteCastMember - Endpoints")]
    public async Task DeleteCastMember()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var targetCastMember = exampleCastMembers[5];
        var untouchedCastMember = exampleCastMembers[0];
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/castmember/{targetCastMember.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var deletedFromDb = await _fixture.Persistence.GetById(targetCastMember.Id);
        deletedFromDb.Should().BeNull();
        var untouchedFromDb = await _fixture.Persistence.GetById(untouchedCastMember.Id);
        untouchedFromDb.Should().NotBeNull();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "CastMember/DeleteCastMember - Endpoints")]
    public async Task WhenNotFound404()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/castmember/{randomGuid}");

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
