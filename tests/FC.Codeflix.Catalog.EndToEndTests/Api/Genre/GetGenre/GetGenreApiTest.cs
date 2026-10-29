using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenre;

[Collection(nameof(GetGenreApiTestFixture))]
public class GetGenreApiTest : IDisposable
{
    private readonly GetGenreApiTestFixture _fixture;
    public GetGenreApiTest(GetGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact(DisplayName = nameof(GetGenre))]
    [Trait("Genre", "Get/GetById - Endpoints")]
    public async Task GetGenre()
    {
        // Arrange
        var genre = _fixture.GetExampleGenre();
        var categories = _fixture.GetExampleCategoriesList(5);
        genre.AddRangeCategories(categories.Select(x => x.Id).ToList());

        var genreCategories = genre.Categories
            .Select(categoryId => new GenresCategories(categoryId, genre.Id))
            .ToList();

        await _fixture.Persistence.Insert(genre);
        await _fixture.Persistence.BulkInsertCategories(categories);
        await _fixture.Persistence.BulkInserGenreCategories(genreCategories);

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<GenreModelOutput>>($"/genres/{genre.Id}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        _fixture.CompareEntityOutput(genre, output.Data);
        _fixture.CompareOutputGenreCategories(output.Data.Categories, categories);
    }


    [Fact(DisplayName = nameof(GetGenreReturnStatus404NotFound))]
    [Trait("Genre", "Get/GetById - Endpoints")]
    public async Task GetGenreReturnStatus404NotFound()
    {
        // Arrange
        var genreId = Guid.NewGuid();

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ProblemDetails>($"/genres/{genreId}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Detail.Should().Be($"Genre with ID {genreId} not found.");
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
