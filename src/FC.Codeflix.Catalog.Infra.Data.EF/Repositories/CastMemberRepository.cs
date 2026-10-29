using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class CastMemberRepository : ICastMemberRepository
{
    private readonly CodeflixCatalogDbContext _context;

    private DbSet<CastMember> _castMembers =>
        _context.Set<CastMember>();

    public CastMemberRepository(CodeflixCatalogDbContext context)
    {
        _context = context;
    }

    public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
        => Task.FromResult(_castMembers.Remove(aggregate));

    public async Task<CastMember> Get(Guid id, CancellationToken cancellation)
    {
        var result = await _castMembers.AsNoTracking().FirstOrDefaultAsync(
           x => x.Id == id,
           cancellation
       );
        NotFoundException.ThrowIfNull(result, id);
        return result!;
    }

    public async Task Insert(CastMember aggregate, CancellationToken cancellation)
        => await _castMembers.AddAsync(aggregate, cancellation);

    public Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
        => Task.FromResult(_castMembers.Update(aggregate));
}
