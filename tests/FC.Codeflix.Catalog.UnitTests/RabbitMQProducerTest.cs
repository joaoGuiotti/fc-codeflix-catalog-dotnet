using FC.Codeflix.Catalog.Domain.Events;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.UnitTests;

public class RabbitMQProducerTest
{
    [Fact(DisplayName = nameof(SendMessageAsync))]
    public async Task SendMessageAsync()
    {
        ConnectionFactory channelFactory = new()
        {
            HostName = "localhost",
            UserName = "adm_videos",
            Password = "123456"
        };

        var connection = await channelFactory.CreateConnectionAsync();
        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true
        );
        var channel = await connection.CreateChannelAsync(channelOptions);

        var options = Options.Create(new RabbitMQConfiguration
        {
            ExchangeName = "video.events"
        });

        var producer = new RabbitMQProducer(channel, options);
        var @event = new VideoUploadedEvent(Guid.NewGuid(), "videos/test.mp4");

        await producer.SendMessageAsync(@event, CancellationToken.None);
    }
}
