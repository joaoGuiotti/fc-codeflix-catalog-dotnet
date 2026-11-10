using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.SeedWork;

public class AggregateRootTest
{
    [Fact(DisplayName = nameof(RaiseEvent))]
    [Trait("Domain", "AggregateRoot - SeedWork")]
    public void RaiseEvent()
    {

        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();

        aggregate.RaiseEvent(domainEvent);
        aggregate.Events.Should().HaveCount(1);
    }

    [Fact(DisplayName = nameof(ClearEvents))]
    [Trait("Domain", "AggregateRoot - SeedWork")]
    public void ClearEvents()
    {
        var domainEvent = new DomainEventFake();
        var aggregate = new AggregateRootFake();
        aggregate.RaiseEvent(domainEvent);
        aggregate.ClearEvents();
        aggregate.Events.Should().HaveCount(0);
    }
}
