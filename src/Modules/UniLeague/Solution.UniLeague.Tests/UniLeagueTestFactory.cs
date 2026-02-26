using Solution.BuildingBlocks.Tests;
using Solution.UniLeague.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Solution.UniLeague.Tests;

public class UniLeagueTestFactory : BaseTestFactory<UniLeagueContext>
{
    protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<UniLeagueContext>));
        if (descriptor != null) services.Remove(descriptor);

        services.AddDbContext<UniLeagueContext>(SetupTestContext());

        return services;
    }
}
