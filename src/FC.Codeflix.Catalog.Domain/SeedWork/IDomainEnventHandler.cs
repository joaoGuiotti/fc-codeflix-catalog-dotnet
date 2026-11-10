namespace FC.Codeflix.Catalog.Domain.SeedWork;

public interface IDomainEnventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken ct);
}
