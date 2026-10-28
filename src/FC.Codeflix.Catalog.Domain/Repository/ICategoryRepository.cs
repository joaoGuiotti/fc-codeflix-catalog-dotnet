using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;

public interface ICategoryRepository
    : IGenericRepository<Category>,
        ISearchableRepository<Category>
{
    public Task<IReadOnlyList<Guid>> GetIdsListById(
        List<Guid> categoriesIds,
        CancellationToken cancellationToken
    );

    public Task<IReadOnlyList<Category>> GetListByIds(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    );
}
