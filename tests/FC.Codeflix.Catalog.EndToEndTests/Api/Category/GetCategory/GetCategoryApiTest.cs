using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;

[Collection(nameof(GetCategoryApiTestFixtureColletion))]
public class GetCategoryApiTest
{

    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiTest(GetCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task GetCategory()
    {
        // Arrange
        var exampleCategories = _fixture.GetExampleCategoryList();
        await _fixture.Persistence.BulkInsert(exampleCategories.ToList());

        var category = exampleCategories.First();

        // act
        var (response, output) = await _fixture.ApiClient
            .Get<CategoryModelOutput>($"/categories/{category.Id}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Id.Should().Be(category.Id);
        output.Name.Should().Be(category.Name);
        output.Description.Should().Be(category.Description);
        output.IsActive.Should().Be(category.IsActive);
        output.CreatedAt.Should().Be(category.CreatedAt);
    }


    [Fact(DisplayName = nameof(GetCategoryReturnStatusNotFound))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
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
        output.Detail.Should().Be($"Category '{newGuid}' not found.");
    }

}
