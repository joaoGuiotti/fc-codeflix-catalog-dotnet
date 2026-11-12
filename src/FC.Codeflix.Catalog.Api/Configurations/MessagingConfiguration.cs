using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using FC.Codeflix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class MessagingConfiguration
{
    public static IServiceCollection AddRabbitMQ(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<RabbitMQConfiguration>(
            configuration.GetSection(RabbitMQConfiguration.ConfigurationSection)
        );

        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
            ConnectionFactory factory = new()
            {
                HostName = config.Hostname,
                UserName = config.Username,
                Password = config.Password,
                Port = config.Port
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddSingleton<ChannelManager>();

        return services;
    }

    public static IServiceCollection AddMessageProducer(this IServiceCollection services)
    {
        services.AddTransient<IMessageProducer>(sp =>
        {
            var channelManager = sp.GetRequiredService<ChannelManager>();
            var config = sp.GetRequiredService<IOptions<RabbitMQConfiguration>>();
            var channel = channelManager.GetChannel().GetAwaiter().GetResult();
            return new RabbitMQProducer(channel, config);
        });
        return services;
    }
}
