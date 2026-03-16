using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Solution.BuildingBlocks.Infrastructure.Database;
using Solution.UniLeague.API.Public;
using Solution.UniLeague.Core.Domain.RepositoryInterfaces;
using Solution.UniLeague.Core.Mappers;
using Solution.UniLeague.Core.RepositoryInterfaces;
using Solution.UniLeague.Core.UseCases;
using Solution.UniLeague.Infrastructure.Database;
using Solution.UniLeague.Infrastructure.Database.Repositories;
using Solution.UniLeague.Infrastructure.Database.Services;

namespace Solution.UniLeague.Infrastructure;

public static class UniLeagueStartup
{
    public static IServiceCollection ConfigureUniLeagueModule(this IServiceCollection services)
    {
        
        services.AddAutoMapper(typeof(UniLeagueProfile).Assembly);

        SetupCore(services);
        SetupInfrastructure(services);

        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
       
        services.AddScoped<IHealthService, HealthService>();        
        services.AddScoped<IStandingsService, StandingsService>();
        services.AddScoped<ILeagueService, LeagueService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<ITopScorerQueryService, TopScorerQueryService>();

    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        
        services.AddScoped<IMatchRepository, MatchDbRepository>();
        services.AddScoped<ITeamRepository, TeamDbRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            DbConnectionStringBuilder.Build("unileague")); 
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<UniLeagueContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "unileague")));

        
        services.AddScoped<ILeagueRepository, LeagueDbRepository>();
    }
}