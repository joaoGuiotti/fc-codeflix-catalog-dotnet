using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiConnections(builder.Configuration)
    .AddStorage(builder.Configuration)
    .AddRabbitMQ(builder.Configuration)
    .AddUseCases()
    .AddMessageProducer()
    .AddMessageConsumer()
    .AddControllersAndConfigure();

var app = builder.Build();

app.StartWebApplicationsMigrations();

app.useDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
