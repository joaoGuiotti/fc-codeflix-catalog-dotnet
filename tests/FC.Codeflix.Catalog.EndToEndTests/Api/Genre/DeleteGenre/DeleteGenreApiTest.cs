using System.Net;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteGenre;

[Collection(nameof(DeleteGenreApiTestFixture))]
public class DeleteGenreApiTest : IDisposable
{
    private readonly DeleteGenreApiTestFixture _fixture;

    public DeleteGenreApiTest(DeleteGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteGenre))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().BeNull();

        var genreFromDb = await _fixture.Persistence.GetById(targetGenre.Id);
        genreFromDb.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task DeleteGenreWithRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        var exampleCategories = _fixture.GetExampleCategoriesList(5);
        targetGenre.AddRangeCategories(exampleCategories.Select(x => x.Id).ToList());
        var genresCategories = targetGenre.Categories
            .Select(categoryId => new GenresCategories(categoryId, targetGenre.Id))
            .ToList();

        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().BeNull();

        var genreFromDb = await _fixture.Persistence.GetById(targetGenre.Id);
        genreFromDb.Should().BeNull();
        var relationsFromDb = await _fixture.Persistence
            .GetGenresCategoriesRelationsByGenreId(targetGenre.Id);
        relationsFromDb.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(WhenNotFound404))]
    [Trait("EndToEnd/API", "Genre/DeleteGenre - Endpoints")]
    public async Task WhenNotFound404()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/genres/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Genre with ID {randomGuid} not found.");
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}

