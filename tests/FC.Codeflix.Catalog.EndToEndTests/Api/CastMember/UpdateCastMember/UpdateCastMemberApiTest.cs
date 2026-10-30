using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.CastMembers;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.UpdateCastMember;

[Collection(nameof(UpdateCastMemberApiTestFixture))]
public class UpdateCastMemberApiTest : IDisposable
{
    private readonly UpdateCastMemberApiTestFixture _fixture;

    public UpdateCastMemberApiTest(UpdateCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCastMember))]
    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    public async Task UpdateCastMember()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var targetCastMember = exampleCastMembers[5];
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var newName = _fixture.GetValidName();
        var newType = _fixture.GetDifferentType(targetCastMember.Type);
        var request = new UpdateCastMemberRequest(newName, newType);

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<CastMemberOutput>>($"/castmember/{targetCastMember.Id}", request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetCastMember.Id);
        output.Data.Name.Should().Be(newName);
        output.Data.type.Should().Be(newType);
        output.Data.CreatedAt.Should().BeSameDateAs(targetCastMember.CreatedAt);

        var castMemberFromDb = await _fixture.Persistence.GetById(targetCastMember.Id);
        castMemberFromDb.Should().NotBeNull();
        _fixture.CompareEntityOutput(castMemberFromDb!, output.Data);
    }

    [Fact(DisplayName = nameof(UpdateCastMemberOnlyName))]
    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    public async Task UpdateCastMemberOnlyName()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var targetCastMember = exampleCastMembers[5];
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var newName = _fixture.GetValidName();
        var request = new UpdateCastMemberRequest(newName, null);

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<CastMemberOutput>>($"/castmember/{targetCastMember.Id}", request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetCastMember.Id);
        output.Data.Name.Should().Be(newName);
        output.Data.type.Should().Be(targetCastMember.Type);
        output.Data.CreatedAt.Should().BeSameDateAs(targetCastMember.CreatedAt);

        var castMemberFromDb = await _fixture.Persistence.GetById(targetCastMember.Id);
        castMemberFromDb.Should().NotBeNull();
        _fixture.CompareEntityOutput(castMemberFromDb!, output.Data);
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var request = _fixture.GetExampleRequest();

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/castmember/{randomGuid}", request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Detail.Should().Be($"CastMember with ID {randomGuid} not found.");
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateCastMember))]
    [Trait("EndToEnd/API", "CastMember/UpdateCastMember - Endpoints")]
    [MemberData(
        nameof(UpdateCastMemberApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCastMemberApiTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdateCastMember(
        UpdateCastMemberRequest request,
        string expectedMessage
    )
    {
        var exampleCastMembers = _fixture.GetExampleCastMembersList(10);
        var targetCastMember = exampleCastMembers[5];
        await _fixture.Persistence.BulkInsert(exampleCastMembers);

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/castmember/{targetCastMember.Id}", request);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred.");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be(expectedMessage);
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
