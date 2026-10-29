using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[Collection(nameof(CreateGenreApiTestFixture))]
public class CreateGenreApiTest : IDisposable
{
    private readonly CreateGenreApiTestFixture _fixture;

    public CreateGenreApiTest(CreateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenre()
    {
        var input = _fixture.GetExampleInput();

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Categories.Should().HaveCount(0);

        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(output.Data.Name);
        genreFromDb.IsActive.Should().Be(output.Data.IsActive);
        genreFromDb.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);
    }

    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    public async Task CreateGenreWithRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        var relatedCategories = exampleCategories.Select(x => x.Id).ToList();
        var input = _fixture.GetExampleInput(relatedCategories);

        var (response, output) = await _fixture.ApiClient
            .Post<ApiResponse<GenreModelOutput>>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);
        var outputRelatedCategoryIds = output.Data.Categories.Select(x => x.Id).ToList();
        outputRelatedCategoryIds.Should().BeEquivalentTo(relatedCategories);

        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(output.Data.Name);
        genreFromDb.IsActive.Should().Be(output.Data.IsActive);
        genreFromDb.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var relationsFromDb = await _fixture.Persistence
            .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        var relatedCategoriesIdsFromDb = relationsFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);
    }

    [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    public async Task ErrorWithInvalidRelations()
    {
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        var relatedCategories = exampleCategories.Select(x => x.Id).ToList();
        var invalidCategoryId = Guid.NewGuid();
        relatedCategories.Add(invalidCategoryId);
        var input = _fixture.GetExampleInput(relatedCategories);

        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/genres", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Invalid Related Aggregate");
        output.Type.Should().Be("RelatedAggregate");
        output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be($"Related Category id (or ids) not found: {invalidCategoryId}.");
    }

    [Theory(DisplayName = nameof(ThrowWhenCantCreateGenre))]
    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    [MemberData(
        nameof(CreateGenreApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreateGenreApiTestDataGenerator)
    )]
    public async Task ThrowWhenCantCreateGenre(
        CreateGenreInput input,
        string expectedMessage
    )
    {
        var (response, output) = await _fixture.ApiClient
            .Post<ProblemDetails>("/genres", input);

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

