using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixtureColletion))]
public class UpdateCategoryApiTest : IDisposable
{
    private readonly UpdateCategoryApiTestFixture _fixture;

    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var category = _fixture.GetExampleCategory();
        await _fixture.Persistence.Add(category);

        var input = _fixture.GetExampleInput(category.Id);
        var (response, output) = await _fixture.ApiClient
            .Put<ApiResponse<CategoryModelOutput>>($"/categories/{category.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        var dbCategory = await _fixture.Persistence.GetById(output.Data.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Data.Id);
        dbCategory.Name.Should().Be(output.Data.Name);
        dbCategory.Description.Should().Be(output.Data.Description);
        dbCategory.IsActive.Should().Be(output.Data.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        dbCategory.CreatedAt.Should().BeSameDateAs(output.Data.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateCategoryReturnStatusNotFound))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategoryReturnStatusNotFound()
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

    [Theory(DisplayName = nameof(ThrowAggregateUpdateError))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [MemberData(
        nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateCategoryApiTestDataGenerator)
    )]
    public async Task ThrowAggregateUpdateError(
        UpdateCategoryInput input,
        string expectedMessage
    )
    {
        var categories = _fixture.GetExampleCategoryList();
        await _fixture.Persistence.BulkInsert(categories.ToList());

        var category = categories.ElementAt(10);

        var (response, output) = await _fixture.ApiClient
            .Put<ProblemDetails>($"/categories/{category.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
        output.Should().NotBeNull();
        output.Title.Should().Be("One or more validation errors occurred.");
        output.Status.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        output.Detail.Should().Be(expectedMessage);
    }

    void IDisposable.Dispose()
        => _fixture.DisposePersistence();
}
