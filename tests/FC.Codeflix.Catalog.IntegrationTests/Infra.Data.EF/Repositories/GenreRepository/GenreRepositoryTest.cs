using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.GenreRepository;

[Collection(nameof(GenreRepositoryTestFixture))]
public class GenreRepositoryTest
{
    private readonly GenreRepositoryTestFixture _fixture;

    public GenreRepositoryTest(GenreRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Insert()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var genreRepository = new Repository.GenreRepository(dbContext);

        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(InsertWithCategoriesRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task InsertWithCategoriesRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(4);
        exampleCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(exampleCategories);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(dbContext);
        await genreRepository.Insert(exampleGenre, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext.Genres.FindAsync(exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var relations = await assertsDbContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(exampleCategories.Count);
        var relatedCategoriesIds = relations.Select(x => x.CategoryId).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(exampleGenre.Categories);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Get()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.Genres.AddAsync(exampleGenre);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var dbGenre = await genreRepository.Get(
            exampleGenre.Id,
            CancellationToken.None
        );

        dbGenre.Should().NotBeNull();
        dbGenre.Id.Should().Be(exampleGenre.Id);
        dbGenre.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
    }

    [Fact(DisplayName = nameof(GetWithCategoriesRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetWithCategoriesRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(4);
        exampleCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(exampleCategories);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var dbGenre = await genreRepository.Get(
            exampleGenre.Id,
            CancellationToken.None
        );

        dbGenre.Should().NotBeNull();
        dbGenre.Id.Should().Be(exampleGenre.Id);
        dbGenre.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);
        dbGenre.Categories.Should().HaveCount(exampleCategories.Count);
        dbGenre.Categories.Should().BeEquivalentTo(exampleGenre.Categories);
    }

    [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task GetThrowsWhenNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(dbContext);
        var randomGuid = Guid.NewGuid();

        var action = async () =>
            await genreRepository.Get(randomGuid, CancellationToken.None);

        await action
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Genre with ID {randomGuid} not found.");
    }

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Delete()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(4);
        exampleCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(exampleCategories);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        await genreRepository.Delete(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext
            .Genres.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);
        dbGenre.Should().BeNull();

        var relations = await assertsDbContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        relations.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task Update()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var exampleCategories = _fixture.GetExampleCategoriesList(4);
        exampleCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(exampleCategories);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        if (exampleGenre.IsActive)
            exampleGenre.Deactivate();
        else
            exampleGenre.Activate();

        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext
            .Genres.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var relations = await assertsDbContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(exampleCategories.Count);
        var relatedCategoriesIds = relations.Select(x => x.CategoryId).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(exampleGenre.Categories);
    }

    [Fact(DisplayName = nameof(UpdateReplacingCategoriesRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateReplacingCategoriesRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var initialCategories = _fixture.GetExampleCategoriesList(4);
        var newCategories = _fixture.GetExampleCategoriesList(3);
        initialCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(initialCategories);
        await dbContext.Categories.AddRangeAsync(newCategories);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        exampleGenre.RemoveAllCategories();
        newCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );

        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var dbGenre = await assertsDbContext
            .Genres.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == exampleGenre.Id);

        dbGenre.Should().NotBeNull();
        dbGenre!.Name.Should().Be(exampleGenre.Name);
        dbGenre.IsActive.Should().Be(exampleGenre.IsActive);
        dbGenre.CreatedAt.Should().Be(exampleGenre.CreatedAt);

        var relations = await assertsDbContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        relations.Should().HaveCount(newCategories.Count);
        var relatedCategoriesIds = relations.Select(x => x.CategoryId).ToList();
        relatedCategoriesIds
            .Should()
            .BeEquivalentTo(newCategories.Select(x => x.Id));
    }

    [Fact(DisplayName = nameof(UpdateRemovingAllCategoriesRelations))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task UpdateRemovingAllCategoriesRelations()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenre = _fixture.GetExampleGenre();
        var initialCategories = _fixture.GetExampleCategoriesList(4);
        initialCategories.ForEach(category =>
            exampleGenre.AddCategory(category.Id)
        );
        await dbContext.Categories.AddRangeAsync(initialCategories);
        await dbContext.Genres.AddAsync(exampleGenre);
        foreach (var categoryId in exampleGenre.Categories)
        {
            var relation = new GenresCategories(categoryId, exampleGenre.Id);
            await dbContext.GenresCategories.AddAsync(relation);
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var actDbContext = _fixture.CreateDbContext(preserveData: true);
        var genreRepository = new Repository.GenreRepository(actDbContext);

        exampleGenre.Update(_fixture.GetValidGenreName());
        exampleGenre.RemoveAllCategories();

        await genreRepository.Update(exampleGenre, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(preserveData: true);
        var relations = await assertsDbContext
            .GenresCategories.Where(x => x.GenreId == exampleGenre.Id)
            .ToListAsync();
        relations.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(SearchReturnListAndTotal))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var exampleItem = exampleGenresList.Find(x =>
                    x.Id == outputItem.Id
                );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            });
    }

    [Fact(DisplayName = nameof(SearchReturnsRelationsWithCategories))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnsRelationsWithCategories()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(15);
        await dbContext.Genres.AddRangeAsync(exampleGenresList);

        foreach (var genre in exampleGenresList)
        {
            var categoriesToAdd = _fixture.GetExampleCategoriesList(
                new Random().Next(1, 5)
            );
            await dbContext.Categories.AddRangeAsync(categoriesToAdd);
            foreach (var category in categoriesToAdd)
            {
                genre.AddCategory(category.Id);
                await dbContext.GenresCategories.AddAsync(
                    new GenresCategories(category.Id, genre.Id)
                );
            }
        }
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);

        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var exampleItem = exampleGenresList.Find(x =>
                    x.Id == outputItem.Id
                );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                outputItem
                    .Categories.Should()
                    .HaveCount(exampleItem.Categories.Count);
                outputItem
                    .Categories.Should()
                    .BeEquivalentTo(exampleItem.Categories);
            });
    }

    [Fact(DisplayName = nameof(SearchReturnEmptyWhenPersistenceIsEmpty))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    public async Task SearchReturnEmptyWhenPersistenceIsEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var genreRepository = new Repository.GenreRepository(dbContext);

        var searchInput = new SearchInput(1, 20, "", "", ESearchOrder.Asc);
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = nameof(SearchWithPagination))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchWithPagination(
        int quantityGenresToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresList(
            quantityGenresToGenerate
        );
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(
            page,
            perPage,
            "",
            "",
            ESearchOrder.Asc
        );
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Items.Should().OnlyHaveUniqueItems();

        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var exampleItem = exampleGenresList.Find(x =>
                    x.Id == outputItem.Id
                );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            });
    }

    [Theory(DisplayName = nameof(SearchWithSearchFilter))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("Action", ESearchOrder.Asc, 1, 5, 3, 3)]
    [InlineData("Horror", ESearchOrder.Desc, 1, 5, 2, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 2, 1, 1, 2)]
    [InlineData("Horror", ESearchOrder.Desc, 3, 1, 0, 2)]
    [InlineData("Drama", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Comedy", ESearchOrder.Asc, 1, 3, 2, 2)]
    [InlineData("Sci-fi", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("Romance", ESearchOrder.Asc, 1, 5, 1, 1)]
    [InlineData("NotFound", ESearchOrder.Asc, 1, 5, 0, 0)]
    public async Task SearchWithSearchFilter(
        string searchTerm,
        ESearchOrder sortOrder,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresListByNames(
            new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Action",
                "Drama",
                "Action Comedy",
                "Comedy",
                "Sci-fi",
                "Romance",
            }
        );
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(
            page,
            perPage,
            searchTerm,
            "",
            sortOrder
        );
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Items.Should().OnlyHaveUniqueItems();

        output
            .Items.ToList()
            .ForEach(outputItem =>
            {
                var exampleItem = exampleGenresList.Find(x =>
                    x.Id == outputItem.Id
                );
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem!.Name);
                outputItem.IsActive.Should().Be(exampleItem.IsActive);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            });
    }

    [Theory(DisplayName = nameof(SearchWithOrderByAndOrder))]
    [Trait("Integration/Infra.Data", "GenreRepository - Repositories")]
    [InlineData("name", ESearchOrder.Asc)]
    [InlineData("name", ESearchOrder.Desc)]
    [InlineData("createdAt", ESearchOrder.Asc)]
    [InlineData("createdAt", ESearchOrder.Desc)]
    [InlineData("", ESearchOrder.Asc)]
    [InlineData("", ESearchOrder.Desc)]
    public async Task SearchWithOrderByAndOrder(
        string orderBy,
        ESearchOrder searchOrder
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
        var exampleGenresList = _fixture.GetExampleGenresListByNames(
            new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Action",
                "Drama",
                "Action Comedy",
                "Comedy",
            }
        );
        await dbContext.Genres.AddRangeAsync(exampleGenresList);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var genreRepository = new Repository.GenreRepository(
            _fixture.CreateDbContext(preserveData: true)
        );

        var searchInput = new SearchInput(1, 10, "", orderBy, searchOrder);
        var output = await genreRepository.Search(
            searchInput,
            CancellationToken.None
        );

        output.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleGenresList.Count);
        output.Items.Should().HaveCount(exampleGenresList.Count);
        output.Items.Should().OnlyHaveUniqueItems();

        var expectedOrderedList = (orderBy.ToLower(), searchOrder) switch
        {
            ("name", ESearchOrder.Asc) => exampleGenresList
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList(),
            ("name", ESearchOrder.Desc) => exampleGenresList
                .OrderByDescending(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList(),
            ("createdat", ESearchOrder.Asc) => exampleGenresList
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            ("createdat", ESearchOrder.Desc) => exampleGenresList
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            (_, ESearchOrder.Asc) => exampleGenresList
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
            _ => exampleGenresList
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToList(),
        };

        for (int i = 0; i < output.Items.Count; i++)
        {
            output.Items[i].Id.Should().Be(expectedOrderedList[i].Id);
            output.Items[i].Name.Should().Be(expectedOrderedList[i].Name);
            output
                .Items[i]
                .IsActive.Should()
                .Be(expectedOrderedList[i].IsActive);
            output
                .Items[i]
                .CreatedAt.Should()
                .Be(expectedOrderedList[i].CreatedAt);
        }
    }
}

