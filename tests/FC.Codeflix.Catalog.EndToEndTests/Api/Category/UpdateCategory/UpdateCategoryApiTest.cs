using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixtureColletion))]
public class UpdateCategoryApiTest
{
    private readonly UpdateCategoryApiTestFixture _fixture;

    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void UpdateCategory()
    {
        var category = _fixture.GetExampleCategory();
        await _fixture.Persistence.Add(category);

        var input = _fixture.GetExampleInput(category.Id);
        var (response, output) = await _fixture.ApiClient
            .Put<CategoryModelOutput>($"/categories/{category.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));

        var dbCategory = await _fixture.Persistence.GetById(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(output.Id);
        dbCategory.Name.Should().Be(output.Name);
        dbCategory.Description.Should().Be(output.Description);
        dbCategory.IsActive.Should().Be(output.IsActive);
        dbCategory.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
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
        output.Detail.Should().Be($"Category '{newGuid}' not found.");
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
}
