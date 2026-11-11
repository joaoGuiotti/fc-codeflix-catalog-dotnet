using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.SeedWork;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.Infra.Data.EF.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CodeflixCatalogDbContext _context;
    private readonly IDomainEventPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        CodeflixCatalogDbContext context,
        IDomainEventPublisher publisher,
        ILogger<UnitOfWork> logger
    )
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Commit(CancellationToken cancellation)
    {
        var aggregateRoots = _context.ChangeTracker
             .Entries<AggregateRoot>()
             .Where(e => e.Entity.Events.Any())
             .Select(e => e.Entity);

        _logger.LogInformation($"Commit: {aggregateRoots.Count()} aggregate roots with events.");

        var events = aggregateRoots
            .SelectMany(e => e.Events)
            .ToList();

        _logger.LogInformation($"Commit: {events.Count()} events raised.");

        foreach (var @event in events)
            await _publisher.PublishAsync((dynamic)@event, cancellation);

        foreach (var aggregate in aggregateRoots)
            aggregate.ClearEvents();

        await _context.SaveChangesAsync(cancellation);
    }

    public Task Rollback(CancellationToken cancellation)
        => Task.CompletedTask;
}
