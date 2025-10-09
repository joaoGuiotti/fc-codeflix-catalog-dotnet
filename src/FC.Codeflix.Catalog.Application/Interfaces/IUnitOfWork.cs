namespace FC.Codeflix.Catalog.Application.Interfaces;

public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellation);

    public Task Rollback(CancellationToken cancellation);
}
