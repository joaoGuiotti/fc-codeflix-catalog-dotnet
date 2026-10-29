using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[Collection(nameof(UpdateGenreApiTestFixture))]
public class UpdateGenreApiTest : IDisposable
{
    private readonly UpdateGenreApiTestFixture _fixture;

    public UpdateGenreApiTest(UpdateGenreApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateGenre))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenre()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidName(),
            !targetGenre.IsActive
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.isActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(targetGenre.CreatedAt);
        output.Data.Categories.Should().HaveCount(0);

        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(output.Data.Name);
        genreFromDb.IsActive.Should().Be(output.Data.IsActive);
        genreFromDb.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenreWithRelations()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );
        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var newRelationsCount = random.Next(2, 5);
        var newRelatedCategoriesIds = new List<Guid>();
        for (int i = 0; i < newRelationsCount; i++)
        {
            var randomCategoryIndex = random.Next(0, exampleCategories.Count);
            var selectedCategory = exampleCategories[randomCategoryIndex];
            if (!newRelatedCategoriesIds.Contains(selectedCategory.Id))
                newRelatedCategoriesIds.Add(selectedCategory.Id);
        }

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidName(),
            !targetGenre.IsActive,
            newRelatedCategoriesIds
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.isActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(targetGenre.CreatedAt);
        output.Data.Categories.Should().HaveCount(newRelatedCategoriesIds.Count);
        var relatedCategoryIdsFromOutput = output.Data.Categories.Select(x => x.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(newRelatedCategoriesIds);

        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(output.Data.Name);
        genreFromDb.IsActive.Should().Be(output.Data.IsActive);
        genreFromDb.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var relationsFromDb = await _fixture.Persistence
            .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(newRelatedCategoriesIds.Count);
        var relatedCategoriesIdsFromDb = relationsFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelatedCategoriesIds);
    }

    [Fact(DisplayName = nameof(UpdateGenrePersistingRelationsWhenNotPresentInInput))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task UpdateGenrePersistingRelationsWhenNotPresentInInput()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        var exampleCategories = _fixture.GetExampleCategoriesList(10);
        var random = new Random();
        exampleGenres.ForEach(genre =>
        {
            var categoriesCount = random.Next(2, 5);
            for (int i = 0; i < categoriesCount; i++)
            {
                var randomCategoryIndex = random.Next(0, exampleCategories.Count);
                var selectedCategory = exampleCategories[randomCategoryIndex];
                if (!genre.Categories.Contains(selectedCategory.Id))
                    genre.AddCategory(selectedCategory.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGenres.ForEach(
            genre => genre.Categories.ToList().ForEach(
                categoryId => genresCategories.Add(new GenresCategories(categoryId, genre.Id))
            )
        );
        await _fixture.Persistence.BulkInsert(exampleGenres);
        await _fixture.Persistence.BulkInsertCategories(exampleCategories);
        await _fixture.Persistence.BulkInserGenreCategories(genresCategories);

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidName(),
            !targetGenre.IsActive
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<GenreModelOutput>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be((bool)input.isActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(targetGenre.CreatedAt);
        output.Data.Categories.Should().HaveCount(targetGenre.Categories.Count);
        var relatedCategoryIdsFromOutput = output.Data.Categories.Select(x => x.Id).ToList();
        relatedCategoryIdsFromOutput.Should().BeEquivalentTo(targetGenre.Categories);

        var genreFromDb = await _fixture.Persistence.GetById(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(output.Data.Name);
        genreFromDb.IsActive.Should().Be(output.Data.IsActive);
        genreFromDb.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);

        var relationsFromDb = await _fixture.Persistence
            .GetGenresCategoriesRelationsByGenreId(output.Data.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(targetGenre.Categories.Count);
        var relatedCategoriesIdsFromDb = relationsFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var input = new UpdateGenreInput(
            randomGuid,
            _fixture.GetValidName(),
            true
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/genres/{randomGuid}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be(StatusCodes.Status404NotFound);
        output.Detail.Should().Be($"Genre with ID {randomGuid} not found.");
    }

    [Fact(DisplayName = nameof(ErrorWhenInvalidRelation))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    public async Task ErrorWhenInvalidRelation()
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        var invalidCategoryId = Guid.NewGuid();
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var input = new UpdateGenreInput(
            targetGenre.Id,
            _fixture.GetValidName(),
            !targetGenre.IsActive,
            new List<Guid> { invalidCategoryId }
        );

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Invalid Related Aggregate");
        output.Type.Should().Be("RelatedAggregate");
        output.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be($"Related Category id (or ids) not found: {invalidCategoryId}.");
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateGenre))]
    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [MemberData(
        nameof(UpdateGenreApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateGenreApiTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdateGenre(
        UpdateGenreInput input,
        string expectedMessage
    )
    {
        var exampleGenres = _fixture.GetExampleGenreList(10);
        var targetGenre = exampleGenres[5];
        await _fixture.Persistence.BulkInsert(exampleGenres);

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

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

