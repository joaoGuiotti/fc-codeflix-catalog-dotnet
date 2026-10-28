using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Category> _categories => _context.Set<Category>();

    public CategoryRepository(CodeflixCatalogDbContext context) =>
        _context = context;

    public async Task Insert(
        Category aggregate,
        CancellationToken cancellation
    ) => await _categories.AddAsync(aggregate, cancellation);

    public Task Delete(
        Category aggregate,
        CancellationToken cancellationToken
    ) => Task.FromResult(_categories.Remove(aggregate));

    public async Task<Category> Get(Guid id, CancellationToken cancellation)
    {
        var result = await _categories.FindAsync(
            new object[] { id },
            cancellation
        );
        NotFoundException.ThrowIfNull(result, id);
        return result!;
    }

    public async Task<SearchOutput<Category>> Search(
        SearchInput input,
        CancellationToken cancellationToken
    )
    {
        var query = _categories.AsNoTracking()
            .ApplyFilter(
                search: input.Search,
                x => x.Name,
                x => x.Description
            ).ApplyOrder(
                sort: input.Sort,
                sortDir: input.SortDir,
                defaultOrder: x => x.CreatedAt,
                x => x.Id,
                x => x.Name,
                x => x.Description,
                x => x.CreatedAt
            );

        return await query.ToSearchOutputAsync(input, cancellationToken);

        //query = ApplyFilter(query, input.Search);
        //query = ApplyOrder(query, input.Sort!, input.SortDir);

        //var total = await query.CountAsync(cancellationToken);
        //var items = await query
        //    .Skip((input.Page - 1) * input.PerPage)
        //    .Take(input.PerPage)
        //    .ToListAsync();

        //return new SearchOutput<Category>(
        //    input.PerPage,
        //    input.Page,
        //    total,
        //    items.AsReadOnly()
        //);
    }

    public Task Update(
        Category aggregate,
        CancellationToken cancellationToken
    ) => Task.FromResult(_categories.Update(aggregate));

    //private IQueryable<Category> ApplyFilter(
    //    IQueryable<Category> query,
    //    string? filter
    //)
    //{
    //    if (string.IsNullOrWhiteSpace(filter))
    //        return query;
    //    var lowerFilter = filter.ToLower();
    //    return query.Where(x =>
    //        x.Name.ToLower().Contains(lowerFilter)
    //        || (
    //            x.Description != null
    //            && x.Description.ToLower().Contains(lowerFilter)
    //        )
    //    );
    //}

    //private IQueryable<Category> ApplyOrder(
    //    IQueryable<Category> query,
    //    string? orderBy,
    //    ESearchOrder? order
    //)
    //{
    //    return (orderBy?.ToLower(), order) switch
    //    {
    //        ("id", ESearchOrder.Asc) => query.OrderBy(x => x.Id),
    //        ("id", ESearchOrder.Desc) => query.OrderByDescending(x => x.Id),
    //        ("name", ESearchOrder.Asc) => query.OrderBy(x => x.Name),
    //        ("name", ESearchOrder.Desc) => query.OrderByDescending(x => x.Name),
    //        ("description", ESearchOrder.Asc) => query.OrderBy(x =>
    //            x.Description
    //        ),
    //        ("description", ESearchOrder.Desc) => query.OrderByDescending(x =>
    //            x.Description
    //        ),
    //        ("createdat", ESearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
    //        ("createdat", ESearchOrder.Desc) => query.OrderByDescending(x =>
    //            x.CreatedAt
    //        ),
    //        _ => query.OrderByDescending(x => x.CreatedAt),
    //    };
    //}

    public async Task<IReadOnlyList<Guid>> GetIdsListById(List<Guid> categoriesIds, CancellationToken cancellationToken)
        => await _categories.AsNoTracking()
            .Where(c => categoriesIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Category>> GetListByIds(List<Guid> categoryIds, CancellationToken cancellationToken)
        => await _categories.AsNoTracking()
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
}
