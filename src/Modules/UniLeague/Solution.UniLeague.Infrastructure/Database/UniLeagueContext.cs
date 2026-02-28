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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("uni_league"); // might change in the future, but for now this is fine
        base.OnModelCreating(modelBuilder);
    }
}
