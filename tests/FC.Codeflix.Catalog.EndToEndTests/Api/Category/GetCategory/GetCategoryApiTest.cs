using System.Net;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;

[Collection(nameof(GetCategoryApiTestFixtureColletion))]
public class GetCategoryApiTest : IDisposable
{

    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/API", "Category/GetById - Endpoints")]
    public async Task GetCategory()
    {
        // Arrange
        var exampleCategories = _fixture.GetExampleCategoryList();
        await _fixture.Persistence.BulkInsert(exampleCategories.ToList());

        var category = exampleCategories.First();

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ApiResponse<CategoryModelOutput>>($"/categories/{category.Id}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        _fixture.CompareEntityOutput(category, output.Data);
    }


    [Fact(DisplayName = nameof(GetCategoryReturnStatusNotFound))]
    [Trait("EndToEnd/API", "Category/GetById - Endpoints")]
    public async Task GetCategoryReturnStatusNotFound()
    {
        // Arrange
        var exampleCategories = _fixture.GetExampleCategoryList(4);
        await _fixture.Persistence.BulkInsert(exampleCategories.ToList());

        var newGuid = Guid.NewGuid().ToString();

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<ProblemDetails>($"/categories/{newGuid}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Category with ID {newGuid} not found.");
    }
    void IDisposable.Dispose()
       => _fixture.DisposePersistence();
}

