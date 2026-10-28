using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Extensions;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Genre> _genres
           => _context.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories
        => _context.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        _genres.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellation)
    {
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);
        NotFoundException.ThrowIfNull(genre, id);
        var categoriesIds = await _genresCategories
            .Where(x => x.GenreId == genre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellation);
        categoriesIds.ForEach(genre.AddCategory);
        return genre;
    }

    public async Task Insert(Genre aggregate, CancellationToken cancellation)
    {
        await _genres.AddAsync(aggregate, cancellation);
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId => new GenresCategories(categoryId, aggregate.Id));
            await _genresCategories.AddRangeAsync(relations, cancellation);
        }
    }

    public async Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        _genres.Update(aggregate);
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId => new GenresCategories(categoryId, aggregate.Id));
            await _genresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public async Task<SearchOutput<Genre>> Search(
        SearchInput input,
        CancellationToken cancellationToken
    )
    {
        var query = _genres.AsNoTracking()
            .ApplyFilter(
                search: input.Search,
                x => x.Name
            ).ApplyOrder(
                sort: input.Sort,
                sortDir: input.SortDir,
                defaultOrder: x => x.CreatedAt,
                x => x.Id,
                x => x.Name,
                x => x.CreatedAt
            );

        var output = await query.ToSearchOutputAsync(input, cancellationToken);

        if (output.Items.Count == 0)
            return output;

        var genresById = output.Items.ToDictionary(genre => genre.Id);
        var genresIds = genresById.Keys.ToList();

        var relations = await _genresCategories
            .AsNoTracking()
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToListAsync(cancellationToken);

        foreach (var relation in relations)
        {
            if (genresById.TryGetValue(relation.GenreId, out var genre))
                genre.AddCategory(relation.CategoryId);
        }

        return output;
    }
}
