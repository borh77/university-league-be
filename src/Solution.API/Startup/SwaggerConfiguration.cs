using Microsoft.OpenApi.Models;

namespace Solution.API.Startup;

public static class SwaggerConfiguration
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UniLeague API",
                Version = "v1"
            });
        });
        return services;
    }
}
