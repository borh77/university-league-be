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
        modelBuilder.Entity<Match>()
           .OwnsOne(m => m.Result, result =>
           {
               result.Property(r => r.HomeScore).HasColumnName("HomeScore");
               result.Property(r => r.AwayScore).HasColumnName("AwayScore");

               // rezultati po četvrtinama u zasebnoj tabeli
               result.OwnsMany(r => r.Quarters, quarter =>
               {
                   quarter.ToTable("QuarterScores");

                   quarter.WithOwner().HasForeignKey("MatchId");
                   quarter.Property<long>("MatchId");

                   quarter.Property(q => q.QuarterNumber);
                   quarter.Property(q => q.HomeScore).HasColumnName("HomeScore");
                   quarter.Property(q => q.AwayScore).HasColumnName("AwayScore");

                   quarter.HasKey("MatchId", "QuarterNumber");
              });

               // rezultati po setovima (odbojka) u zasebnoj tabeli
               result.OwnsMany(r => r.Sets, set =>
               {
                   set.ToTable("SetScores");

                   set.WithOwner().HasForeignKey("MatchId");
                   set.Property<long>("MatchId");
                   
                   set.Property(s => s.SetNumber);
                   set.Property(s => s.HomeScore).HasColumnName("HomeScore");
                   set.Property(s => s.AwayScore).HasColumnName("AwayScore");
                   
                   set.HasKey("MatchId", "SetNumber");
               });

               // Golovi i strelci (fudbal)
               result.OwnsMany(r => r.Goals, goal =>
               {
                   goal.ToTable("GoalEvents");
                   goal.WithOwner().HasForeignKey("MatchId");
                   // Id se sada automatski mapira iz Entity klase
                   goal.Property(g => g.ScorerName).HasMaxLength(200);
                   goal.Property(g => g.TeamName).HasMaxLength(200);
                   goal.Property(g => g.IsHomeTeamGoal);
                   goal.Property(g => g.Minute);
               });
           });

    //    modelBuilder.Entity<Match>()
    //.OwnsOne(m => m.Result, result =>
    //{
    //    result.Property(r => r.HomeScore).HasColumnName("HomeScore");
    //    result.Property(r => r.AwayScore).HasColumnName("AwayScore");

    //    // Četvrtine kao JSON kolona
    //    result.OwnsMany(r => r.Quarters, quarter =>
    //    {
    //        quarter.ToJson();
    //    });
    //});

        modelBuilder.HasDefaultSchema("unileague"); // might change in the future, but for now this is fine
        modelBuilder.ApplyConfiguration(new LeagueConfiguration());
        modelBuilder.ApplyConfiguration(new StandingEntryConfiguration());
        base.OnModelCreating(modelBuilder);

    }
}