using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.Domain.Repository;

public interface IGenreRepository
        : IGenericRepository<Genre>, ISearchableRepository<Genre>
{
    Task<IEnumerable<Guid>> GetIdsListById(List<Guid> genresIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<Genre>> GetListByIds(List<Guid> genresIds, CancellationToken cancellationToken);
}
