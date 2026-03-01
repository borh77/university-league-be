using Microsoft.EntityFrameworkCore;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Infrastructure.Database;

/// <summary>
/// EF Core DbContext for the UniLeague module.
/// Add DbSets and entity configurations in Phase 1.
/// </summary>
public class UniLeagueContext : DbContext
{
    public UniLeagueContext(DbContextOptions<UniLeagueContext> options) : base(options) { }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Match>().OwnsOne(m => m.Result, r =>
        {
            r.Property(x => x.HomeScore).HasColumnName("HomeScore");
            r.Property(x => x.AwayScore).HasColumnName("AwayScore");
        });

        modelBuilder.HasDefaultSchema("uni_league"); // might change in the future, but for now this is fine
        base.OnModelCreating(modelBuilder);
    }
}
