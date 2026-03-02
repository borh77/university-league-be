using Microsoft.EntityFrameworkCore;
using Solution.UniLeague.Core.Domain;
using Solution.UniLeague.Infrastructure.Database.Configurations;


namespace Solution.UniLeague.Infrastructure.Database;


public class UniLeagueContext : DbContext
{
    public UniLeagueContext(DbContextOptions<UniLeagueContext> options) : base(options) { }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }

    public DbSet<League> Leagues { get; set; }
    public DbSet<StandingEntry> StandingEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Match>().OwnsOne(m => m.Result, r =>
        {
            r.Property(x => x.HomeScore).HasColumnName("HomeScore");
            r.Property(x => x.AwayScore).HasColumnName("AwayScore");
        });

        modelBuilder.HasDefaultSchema("unileague"); // might change in the future, but for now this is fine
        modelBuilder.ApplyConfiguration(new LeagueConfiguration());
        modelBuilder.ApplyConfiguration(new StandingEntryConfiguration());
        base.OnModelCreating(modelBuilder);

    }
}