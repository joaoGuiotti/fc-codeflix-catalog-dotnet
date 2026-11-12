using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public class ChannelManager
{
    private readonly IConnection _connection;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IChannel? _channel;

    public ChannelManager(IConnection connection)
        => _connection = connection;

    public async Task<IChannel> GetChannel(CancellationToken cancellationToken = default)
    {

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_channel == null || _channel.IsClosed)
                _channel = await _connection.CreateChannelAsync();
        }
        finally
        {
            _semaphore.Release();
        }
        return _channel;
    }

    public async Task CloseChannel(CancellationToken cancellationToken = default)
    {
        if (_channel is not null && _channel.IsOpen)
            await _channel.DisposeAsync();

        _semaphore.Dispose();
    }
}
