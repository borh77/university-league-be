using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.UseCases;
using Solution.UniLeague.Infrastructure.Database;
using Solution.UniLeague.Infrastructure.Database.Repositories;

namespace Solution.UniLeague.Infrastructure;

public static class UniLeagueStartup
{
    public static IServiceCollection ConfigureUniLeagueModule(this IServiceCollection services)
    {
        // Register AutoMapper profiles from this module's assembly
        services.AddAutoMapper(typeof(UniLeagueProfile).Assembly);

        SetupCore(services);
        SetupInfrastructure(services);

        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IHealthService, HealthService>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<ITeamService, TeamService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        
        services.AddScoped<IMatchRepository, MatchDbRepository>();
        services.AddScoped<ITeamRepository, TeamDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            DbConnectionStringBuilder.Build("uni_league")); //might change later
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<UniLeagueContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "uni_league")));
    }
}
