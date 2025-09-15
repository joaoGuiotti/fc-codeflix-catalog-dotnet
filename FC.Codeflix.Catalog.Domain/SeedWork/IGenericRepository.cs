namespace FC.Codeflix.Catalog.Domain.SeedWork;

public interface IGenericRepository<TAggregate> : IRepository
{
    public Task Insert(TAggregate aggregate, CancellationToken cancellation);
    public Task<TAggregate> Get(Guid id, CancellationToken cancellation);
    Task Delete(TAggregate aggregate, CancellationToken cancellationToken);
    Task Update(TAggregate aggregate, CancellationToken cancellationToken);
}