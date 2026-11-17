using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateMediaStatus;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.DTOs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.Infra.Messaging.Consumer;

public class VideoEncodedEventConsumer : BackgroundService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new JsonSerializerOptions().ConfigureSnakeCaseJson();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<VideoEncodedEventConsumer> _logger;
    private readonly string _queue;
    private readonly IChannel _channel;

    public VideoEncodedEventConsumer(
        IServiceProvider serviceProvider,
        ILogger<VideoEncodedEventConsumer> logger,
        IOptions<RabbitMQConfiguration> configuration,
        IChannel channel
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _queue = configuration.Value.VideoEncodedQueue;
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceived;

        await _channel.BasicConsumeAsync(
            queue: _queue,
            autoAck: false, // False para não remover mensagens que forma perdidas, controle de exclusão das mensagens
            consumer: consumer,
            cancellationToken: stoppingToken
        );

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected during graceful host shutdown
        }
        finally
        {
            _logger.LogWarning("Disposing channel.");
            await _channel.DisposeAsync();
        }
    }

    private async Task OnMessageReceived(
        object? sender,
        BasicDeliverEventArgs eventArgs
    )
    {
        string messageString = string.Empty;
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var rawMessage = eventArgs.Body.ToArray();
            messageString = Encoding.UTF8.GetString(rawMessage);

            _logger.LogDebug(messageString);

            var message = JsonSerializer.Deserialize<VideoEncodedMessageDTO>(
                messageString,
                _jsonSerializerOptions
            );

            var input = GetUpdateMediaStatusInput(message!);
            await mediator.Send(input, CancellationToken.None);

            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        }
        catch (Exception e)
            when (e is EntityValidationException || e is NotFoundException)
        {
            _logger.LogError(e, "There was a bussines error in the message processing: {deleviryTag}, {message}",
                eventArgs.DeliveryTag, messageString);
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "There was a unxpected error in the message processing: {deleviryTag}, {message}",
              eventArgs.DeliveryTag, messageString);
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
        }
    }

    private UpdateMediaStatusInput GetUpdateMediaStatusInput(
        VideoEncodedMessageDTO messageDTO
    )
    {
        if (messageDTO.Video != null)
        {
            return new UpdateMediaStatusInput(
                (Guid)messageDTO.Video.ResourceId,
                EMediaStatus.Completed,
                EncodedPath: messageDTO.Video.FullEncodedVideoFilePath
            );
        }
        return new UpdateMediaStatusInput(
            (Guid)messageDTO.Message!.ResourceId,
            EMediaStatus.Error,
            ErrorMessage: messageDTO.Error
        );
    }

}
