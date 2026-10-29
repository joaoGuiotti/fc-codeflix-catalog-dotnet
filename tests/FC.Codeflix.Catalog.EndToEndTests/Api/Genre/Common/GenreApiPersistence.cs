using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;

public class GenreApiPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenreApiPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entity.Genre?> GetById(Guid id)
    {
        return await _context.Genres
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id);

    }
    public async Task BulkInsert(List<Domain.Entity.Genre> genres)
    {
        await _context.Genres.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
    }

    public async Task Insert(Domain.Entity.Genre genre)
    {
        await _context.Genres.AddAsync(genre);
        await _context.SaveChangesAsync();
    }

    public async Task BulkInsertCategories(List<Domain.Entity.Category> categories)
    {
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public async Task BulkInserGenreCategories(List<GenresCategories> genreCategories)
    {
        await _context.GenresCategories.AddRangeAsync(genreCategories);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
    {
        return await _context.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == id)
            .ToListAsync();
    }
}
