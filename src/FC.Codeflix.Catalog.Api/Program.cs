using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiConnections()
    .AddUseCases()
    .AddControllersAndConfigure();

var app = builder.Build();

app.useDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
