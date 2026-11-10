using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application;

public class DomainEventPublisherTest
{
    [Fact(DisplayName = nameof(PublishAync))]
    [Trait("Application", "DomainEventPublisher")]
    public async Task PublishAync()
    {
        var serviceCollection = new ServiceCollection();

        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToBeHandledFake>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToBeHandledFake>>();
        var eventHandlerMock3 = new Mock<IDomainEventHandler<DomainEventToNotBeHandledFake>>();

        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        serviceCollection.AddSingleton(eventHandlerMock3.Object);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);

        var @event = new DomainEventToBeHandledFake();
        await domainEventPublisher.PublishAsync(@event, CancellationToken.None);

        eventHandlerMock1.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock2.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
        eventHandlerMock3.Verify(x => x.HandleAsync(It.IsAny<DomainEventToNotBeHandledFake>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = nameof(NoActionWhenThereIsNoSubscriber))]
    [Trait("Application", "DomainEventPublisher")]
    public async Task NoActionWhenThereIsNoSubscriber()
    {
        var serviceCollection = new ServiceCollection();
        var eventHandlerMock1 = new Mock<IDomainEventHandler<DomainEventToNotBeHandledFake>>();
        var eventHandlerMock2 = new Mock<IDomainEventHandler<DomainEventToNotBeHandledFake>>();
        serviceCollection.AddSingleton(eventHandlerMock1.Object);
        serviceCollection.AddSingleton(eventHandlerMock2.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var domainEventPublisher = new DomainEventPublisher(serviceProvider);
        var @event = new DomainEventToBeHandledFake();

        await domainEventPublisher.PublishAsync(@event, CancellationToken.None);

        eventHandlerMock1.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandledFake>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
        eventHandlerMock2.Verify(x => x.HandleAsync(
            It.IsAny<DomainEventToNotBeHandledFake>(),
            It.IsAny<CancellationToken>()),
        Times.Never);
    }
}
