using FC.Codeflix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext _context;

    public UnitOfWork(CodeflixCatalogDbContext context) => _context = context;

    public Task Commit(CancellationToken cancellation) =>
        _context.SaveChangesAsync(cancellation);

    public Task Rollback(CancellationToken cancellation) => Task.CompletedTask;
}
