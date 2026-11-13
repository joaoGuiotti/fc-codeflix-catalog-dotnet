using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Infra.Messaging.Producer;

public class RabbitMQProducer : IMessageProducer
{
    private readonly IChannel _channel;
    private readonly string _exchangeName;

    public RabbitMQProducer(
        IChannel channel,
        IOptions<RabbitMQConfiguration> options
    )
    {
        _channel = channel;
        _exchangeName = options.Value.ExchangeName!;
    }

    public async Task SendMessageAsync<T>(
        T message,
        CancellationToken ct
    )
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var jsonOptions = JsonSerializerConfiguration
            .ConfigureSnakeCaseJson(new JsonSerializerOptions());

        var @event = JsonSerializer
            .SerializeToUtf8Bytes(message, jsonOptions);

        await _channel.BasicPublishAsync(
             exchange: _exchangeName,
             routingKey: routingKey,
             body: @event
         );
    }
}
