using FC.Codeflix.Catalog.Application.EventHandlers;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Events;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.EventHandler;

public class SendToEncoderEventHandlerTest
{
    [Fact(DisplayName = nameof(HandleAsync))]
    [Trait("Application", "EventHandler")]
    public async Task HandleAsync()
    {
        var msgProducerMock = new Mock<IMessageProducer>();
        msgProducerMock.Setup(x => x.SendMessageAsync(
            It.IsAny<VideoUploadedEvent>(),
            It.IsAny<CancellationToken>())
        ).Returns(Task.CompletedTask);

        var handler = new SendToEncoderEventHandler(msgProducerMock.Object);
        VideoUploadedEvent @event = new(Guid.NewGuid(), "medias/video.mp4");

        await handler.HandleAsync(@event, CancellationToken.None);

        msgProducerMock.Verify(x =>
            x.SendMessageAsync(@event, CancellationToken.None)
        , Times.Once);

    }
}
