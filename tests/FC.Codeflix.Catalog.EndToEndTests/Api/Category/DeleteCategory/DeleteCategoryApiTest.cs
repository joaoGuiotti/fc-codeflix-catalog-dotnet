
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryApiTestFixtureCollection))]
public class DeleteCategoryApiTest : IDisposable
{
    private readonly DeleteCategoryApiTestFixture _fixture;

    public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async Task DeleteCategory()
    {
        // Arrange
        var exampleCategories = _fixture.GetExampleCategoryList();
        await _fixture.Persistence.BulkInsert(exampleCategories.ToList());

        var category = exampleCategories.First();

        // act
        var (response, output) = await _fixture.ApiClient
            .Delete<object>($"/categories/{category.Id}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();


        // Validação no BD, garantindo exclusao
        var persistenCategory = await _fixture.Persistence.GetById(category.Id);
        persistenCategory.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteCategoryReturnStatusNotFound))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async Task DeleteCategoryReturnStatusNotFound()
    {
        // Arrange
        var exampleCategories = _fixture.GetExampleCategoryList(4);
        await _fixture.Persistence.BulkInsert(exampleCategories.ToList());

        var newGuid = Guid.NewGuid().ToString();

        // act
        var (response, output) = await _fixture.ApiClient
            .Delete<ProblemDetails>($"/categories/{newGuid}");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Detail.Should().Be($"Category '{newGuid}' not found.");
    }
    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
