using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup>,
    IDisposable where TStartup : class
{
    private const string VideoCreatedRoutingKey = "video.created";
    public string VideoEncodedRoutingKey => "video.encoded";
    public string VideoCreatedQueue => "video.created.queue";
    public IChannel? RabbitMQChannel { get; private set; }
    public RabbitMQConfiguration? RabbitMQConfiguration { get; private set; }
    public readonly Mock<StorageClient> StorageClient = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var environment = "e2e";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
        builder.UseEnvironment(environment);

        builder.ConfigureServices(services =>
        {
            var descriptor = services.First(s =>
                s.ServiceType == typeof(StorageClient));

            services.Remove(descriptor);

            services.AddScoped(sp => StorageClient.Object);

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            RabbitMQChannel = scope.ServiceProvider
                .GetService<ChannelManager>()!
                .GetChannel()
                .GetAwaiter()
                .GetResult();

            RabbitMQConfiguration = scope
                .ServiceProvider
                .GetService<IOptions<RabbitMQConfiguration>>()!
                .Value;

            SetupRabbitMQ().GetAwaiter().GetResult();

            var context = scope.ServiceProvider
                .GetService<CodeflixCatalogDbContext>();

            ArgumentNullException.ThrowIfNull(context);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        });

        base.ConfigureWebHost(builder);
    }

    public async Task SetupRabbitMQ()
    {
        var channel = RabbitMQChannel!;
        var exchange = RabbitMQConfiguration!.ExchangeName;

        await channel.ExchangeDeclareAsync(exchange, "direct", true, false, null);
        await channel.QueueDeclareAsync(VideoCreatedQueue, true, false, false, null);
        await channel.QueueBindAsync(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        await channel.QueueDeclareAsync(
            RabbitMQConfiguration.VideoEncodedQueue,
            true, false, false, null
        );
        await channel.QueueBindAsync(
            RabbitMQConfiguration.VideoEncodedQueue,
            exchange, VideoEncodedRoutingKey, null
        );
    }

    private async Task TearDownRabbitMQAsync()
    {
        var channel = RabbitMQChannel!;
        var exchange = RabbitMQConfiguration!.ExchangeName;
        await channel.QueueUnbindAsync(VideoCreatedQueue, exchange, VideoCreatedRoutingKey, null);
        await channel.QueueDeleteAsync(VideoCreatedQueue, false, false);
        await channel.QueueUnbindAsync(RabbitMQConfiguration.VideoEncodedQueue,
            exchange, VideoEncodedRoutingKey, null);
        await channel.QueueDeleteAsync(RabbitMQConfiguration.VideoEncodedQueue, false, false);
        await channel.ExchangeDeleteAsync(exchange, false);
    }

    public override async ValueTask DisposeAsync()
    {
        await TearDownRabbitMQAsync();
        await base.DisposeAsync();
    }
}
