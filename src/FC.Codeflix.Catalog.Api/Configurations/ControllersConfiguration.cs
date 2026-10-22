using FC.Codeflix.Catalog.Api.Filters;
using System.Net.NetworkInformation;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ControllersConfiguration
{
    public static IServiceCollection AddControllersAndConfigure(this IServiceCollection services)
    {
        services.AddControllers(opt
            => opt.Filters.Add(typeof(ApiGlobalExceptionFilter)));
        services.AddDocumentation();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        return services;
    }

    public static IServiceCollection AddDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        return services;
    }

    public static WebApplication useDocumentation(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {

            app.MapOpenApi();
            app.UseSwaggerUI(options
                => options.SwaggerEndpoint("/openapi/v1.json", "Codeflix Catalog API"));
        }
        return app;
    }
}
