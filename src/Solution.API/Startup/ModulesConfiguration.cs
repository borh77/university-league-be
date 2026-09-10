using Solution.Identity.Infrastructure;
using Solution.UniLeague.Infrastructure;

namespace Solution.API.Startup;

public static class ModulesConfiguration
{
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        services.ConfigureUniLeagueModule();
        services.ConfigureIdentityModule();

        return services;
    }
}
