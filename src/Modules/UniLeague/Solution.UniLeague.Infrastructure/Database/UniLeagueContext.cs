using Microsoft.EntityFrameworkCore;

namespace Solution.UniLeague.Infrastructure.Database;

/// <summary>
/// EF Core DbContext for the UniLeague module.
/// Add DbSets and entity configurations in Phase 1.
/// </summary>
public class UniLeagueContext : DbContext
{
    public UniLeagueContext(DbContextOptions<UniLeagueContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("uni_league"); // might change in the future, but for now this is fine
        base.OnModelCreating(modelBuilder);
    }
}
