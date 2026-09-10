using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.Identity.API.Public;
using Solution.Identity.Core.Domain.RepositoryInterfaces;
using Solution.Identity.Core.Mappers;
using Solution.Identity.Core.UseCases;
using Solution.Identity.Infrastructure.Database;
using Solution.Identity.Infrastructure.Database.Repositories;
using Solution.Identity.Infrastructure.Database.Services;

namespace Solution.Identity.Infrastructure;

public static class IdentityStartup
{
    public static IServiceCollection ConfigureIdentityModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(IdentityProfile).Assembly);

        SetupCore(services);
        SetupInfrastructure(services);

        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            DbConnectionStringBuilder.Build("identity"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<IdentityContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "identity")));
    }
}
