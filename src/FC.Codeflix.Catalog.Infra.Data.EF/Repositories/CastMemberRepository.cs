using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Extensions;
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

    public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var query = _castMembers.AsNoTracking()
            .ApplyFilter(
                input.Search,
                x => x.Name,
                x => x.Type.ToString()
            ).ApplyOrder(
                sort: input.Sort,
                sortDir: input.SortDir,
                defaultOrder: x => x.CreatedAt,
                x => x.Id,
                x => x.Name,
                x => x.Type,
                x => x.CreatedAt
            );

        return await query.ToSearchOutputAsync(input, cancellationToken);
    }

    public Task Update(CastMember aggregate, CancellationToken cancellationToken)
        => Task.FromResult(_castMembers.Update(aggregate));
}
