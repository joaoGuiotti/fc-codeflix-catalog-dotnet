namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

public class CategoryPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public CategoryPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<DomainEntity.Category?> GetById(Guid id)
    {
        return await _context.Categories
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id);

    }
    public async Task BulkInsert(List<DomainEntity.Category> categories)
    {
        await _context.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public async Task Add(DomainEntity.Category category)
    {
        await _context.AddAsync(category);
        await _context.SaveChangesAsync();
    }
}
