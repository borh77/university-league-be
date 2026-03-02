using Microsoft.EntityFrameworkCore;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Infrastructure.Database.Configurations;

namespace Solution.UniLeague.Infrastructure.Database;


public class UniLeagueContext : DbContext
{
    public UniLeagueContext(DbContextOptions<UniLeagueContext> options) : base(options) { }

    public DbSet<League> Leagues { get; set; }
    public DbSet<StandingEntry> StandingEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("unileague"); // might change in the future, but for now this is fine
        modelBuilder.ApplyConfiguration(new LeagueConfiguration());
        modelBuilder.ApplyConfiguration(new StandingEntryConfiguration());
    }
}