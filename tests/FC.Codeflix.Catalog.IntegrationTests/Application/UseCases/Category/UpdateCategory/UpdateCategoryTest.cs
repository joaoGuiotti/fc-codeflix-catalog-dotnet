using Bogus;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;


namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixtureCollection))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Case")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategory(
        DomainEntity.Category exampleCateogry,
        UpdateCategoryInput input
    )
    {
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetExampleCategoryList());
        var trackingInfo = await dbContext.AddAsync(exampleCateogry);

        //Confirma salvamento context db
        dbContext.SaveChanges();

        // Remover do track inclusões
        trackingInfo.State = EntityState.Detached;

        var repositoryMock = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new UseCase.UpdateCategory(repositoryMock, unitOfWork);

        var UpdateCategoryInput = _fixture.GetValidInput();
        CategoryModelOutput output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.CreateDbContext(true))
                .Categories.FindAsync(output.Id);

        dbCategory.Should().NotBeNull();
        dbCategory.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive);
    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenNotFoundCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Case")]
    public async Task UpdateThrowsWhenNotFoundCategory()
    {
        var input = _fixture.GetValidInput();
        var context = _fixture.CreateDbContext();
        await context.AddRangeAsync(_fixture.GetExampleCategoryList());
        context.SaveChanges();

        var repo = new CategoryRepository(context);
        var uow = new UnitOfWork(context);

        var useCase = new UseCase.UpdateCategory(repo, uow);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }


    [Theory(DisplayName = nameof(UpdateCategoryThrowsInvalid))]
    [Trait("Integration/Application", "UpdateCategory - Use Case")]
    [MemberData(
        nameof(UpdateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 5,
        MemberType = typeof(UpdateCategoryTestDataGenerator)
    )]
    public async Task UpdateCategoryThrowsInvalid(
        UpdateCategoryInput input,
        string errorMessage
    )
    {
        var context = _fixture.CreateDbContext();
        var repo = new CategoryRepository(context);
        var uow = new UnitOfWork(context);
        var examples = _fixture.GetExampleCategoryList();
        await context.AddRangeAsync(examples);
        context.SaveChanges();

        var useCase = new UseCase.UpdateCategory(repo, uow);

        input.Id = examples[0].Id;
        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(errorMessage);
    }

}
