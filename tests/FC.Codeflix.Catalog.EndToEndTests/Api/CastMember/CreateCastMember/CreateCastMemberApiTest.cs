using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.CreateCastMember;

[Collection(nameof(CreateCastMemberApiTestFixture))]
public class CreateCastMemberApiTest : IDisposable
{
    private readonly CreateCastMemberApiTestFixture _fixture;

    public CreateCastMemberApiTest(CreateCastMemberApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(CreateCastMember))]
    [Trait("EndToEnd/API", "CastMember/CreateCastMember - Endpoints")]
    [InlineData(ECastMemberType.Actor)]
    [InlineData(ECastMemberType.Director)]
    public async Task CreateCastMember(ECastMemberType type)
    {
        var input = _fixture.GetExampleInput(type);

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<CastMemberOutput>>("/castmember", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.type.Should().Be(input.Type);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var castMemberFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        castMemberFromDb.Should().NotBeNull();
        _fixture.CompareEntityOutput(castMemberFromDb!, output.Data);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantCreateCastMember))]
    [Trait("EndToEnd/API", "CastMember/CreateCastMember - Endpoints")]
    [MemberData(
        nameof(CreateCastMemberApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateCastMemberApiTestDataGenerator)
    )]
    public async Task ThrowWhenCantCreateCastMember(
        CreateCastMemberInput input,
        string expectedMessage
    )
    {
        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/castmember", input);

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
