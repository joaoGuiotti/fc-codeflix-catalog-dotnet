using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoryTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoryTest(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(List))]
    [Trait("Application", "ListCategory - Use Cases")]
    public async Task List()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetValidInput();
        var categories = _fixture.GetCategories();

        repoMock
            .Setup(x =>
                x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new SearchOutput<Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    total: categories.Count,
                    items: (IReadOnlyList<Category>)categories
                )
            );

        var useCase = new UseCase.ListCategories(repoMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(categories.Count);
        output.Total.Should().Be(categories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var category = categories.Find(x => x.Id == outputItem.Id);
                category.Should().NotBeNull();
                outputItem.Name.Should().Be(category!.Name);
                outputItem.Description.Should().Be(category.Description);
                outputItem.IsActive.Should().Be(category.IsActive);
                outputItem.CreatedAt.Should().Be(category.CreatedAt);
            });

        repoMock.Verify(
            x =>
                x.Search(
                    It.Is<SearchInput>(s =>
                        s.Page == input.Page
                        && s.PerPage == input.PerPage
                        && s.Search == input.Search
                        && s.Sort == input.Sort
                        && s.SortDir == input.SortDir
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact(DisplayName = nameof(ListOkWhenIsEmpty))]
    [Trait("Application", "ListCategory - Use Cases")]
    public async Task ListOkWhenIsEmpty()
    {
        var repoMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetValidInput();

        repoMock
            .Setup(x =>
                x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new SearchOutput<Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    items: new List<Category>(0).AsReadOnly(),
                    total: 0
                )
            );

        var useCase = new UseCase.ListCategories(repoMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(0);
        output.Total.Should().Be(0);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);

        repoMock.Verify(
            x =>
                x.Search(
                    It.Is<SearchInput>(s =>
                        s.Page == input.Page
                        && s.PerPage == input.PerPage
                        && s.Search == input.Search
                        && s.Sort == input.Sort
                        && s.SortDir == input.SortDir
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(ListInputWithoutAllProperties))]
    [Trait("Application", "ListCategory - Use Cases")]
    [MemberData(
        nameof(ListCategoriesTestDataGenerator.GetInputsWithoutAllParameters),
        MemberType = typeof(ListCategoriesTestDataGenerator)
    )]
    public async Task ListInputWithoutAllProperties(
        UseCase.ListCategoriesInput input
    )
    {
        var repoMock = _fixture.GetRepositoryMock();
        var categories = _fixture.GetCategories();

        repoMock
            .Setup(x =>
                x.Search(It.IsAny<SearchInput>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(
                new SearchOutput<Category>(
                    currentPage: input.Page,
                    perPage: input.PerPage,
                    total: categories.Count,
                    items: (IReadOnlyList<Category>)categories
                )
            );

        var useCase = new UseCase.ListCategories(repoMock.Object);
        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().HaveCount(categories.Count);
        output.Total.Should().Be(categories.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var category = categories.Find(x => x.Id == outputItem.Id);
                category.Should().NotBeNull();
                outputItem.Name.Should().Be(category!.Name);
                outputItem.Description.Should().Be(category.Description);
                outputItem.IsActive.Should().Be(category.IsActive);
                outputItem.CreatedAt.Should().Be(category.CreatedAt);
            });

        repoMock.Verify(
            x =>
                x.Search(
                    It.Is<SearchInput>(s =>
                        s.Page == input.Page
                        && s.PerPage == input.PerPage
                        && s.Search == input.Search
                        && s.Sort == input.Sort
                        && s.SortDir == input.SortDir
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
