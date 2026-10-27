using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
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
    {
        throw new NotImplementedException();
    }

    public async Task<Genre> Get(Guid id, CancellationToken cancellation)
    {
        var result = await _genres.FindAsync(
            [id],
            cancellation
        );
        NotFoundException.ThrowIfNull(result, $"Genre '{id}' not found.");
        return result!;
    }

    public async Task Insert(Genre aggregate, CancellationToken cancellation)
        => await _genres.AddAsync(aggregate, cancellation);

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
