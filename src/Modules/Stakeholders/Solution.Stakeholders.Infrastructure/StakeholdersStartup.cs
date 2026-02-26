using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.Stakeholders.API.Public;
using Solution.Stakeholders.Core.Domain.RepositoryInterfaces;
using Solution.Stakeholders.Core.Mappers;
using Solution.Stakeholders.Core.UseCases;
using Solution.Stakeholders.Infrastructure.Authentication;
using Solution.Stakeholders.Infrastructure.Database;
using Solution.Stakeholders.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Solution.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }
    
    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
