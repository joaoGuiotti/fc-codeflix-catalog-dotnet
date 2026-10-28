using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Genre> _genres
           => _context.Set<Genre>();

    public GenreRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
        => Task.FromResult(_context.Remove(aggregate));

    public async Task<Genre> Get(Guid id, CancellationToken cancellation)
    {
        var result = await _genres.FindAsync(
            [id],
            cancellation
        );
        NotFoundException.ThrowIfNull(result, id);
        return result!;
    }

    public async Task Insert(Genre aggregate, CancellationToken cancellation)
        => await _genres.AddAsync(aggregate, cancellation);

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
                x => x.Name,
                x => x.CreatedAt
            );
        return await query.ToSearchOutputAsync(input, cancellationToken);
    }
}
