using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;

public class CastMemberApiPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public CastMemberApiPersistence(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public async Task<DomainEntity.CastMember?> GetById(Guid id)
        => await _context.CastMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task Insert(DomainEntity.CastMember castMember)
    {
        await _context.CastMembers.AddAsync(castMember);
        await _context.SaveChangesAsync();
    }

    public async Task BulkInsert(List<DomainEntity.CastMember> castMembers)
    {
        await _context.CastMembers.AddRangeAsync(castMembers);
        await _context.SaveChangesAsync();
    }
}
