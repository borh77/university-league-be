using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Infrastructure.Database.Configurations;

public class StandingEntryConfiguration : IEntityTypeConfiguration<StandingEntry>
{
    public void Configure(EntityTypeBuilder<StandingEntry> builder)
    {
        builder.ToTable("Standings");

        
        builder.HasKey(s => s.Id);

        
        builder.HasIndex(s => new { s.LeagueId, s.TeamId }).IsUnique();

        builder.Property(s => s.TeamName).IsRequired().HasMaxLength(200);
        builder.Property(s => s.LogoUrl).IsRequired(false);
        builder.Property(s => s.Played).IsRequired();
        builder.Property(s => s.Won).IsRequired();
        builder.Property(s => s.Drawn).IsRequired();  
        builder.Property(s => s.Lost).IsRequired();
        builder.Property(s => s.Points).IsRequired();
        builder.Property(s => s.Scored).IsRequired();
        builder.Property(s => s.Conceded).IsRequired();
        builder.Property(s => s.SetWon).IsRequired(false);
        builder.Property(s => s.SetLost).IsRequired(false);

        // Might change in the future if we decide to calculate these values in the database instead of in the application
        builder.Ignore(s => s.Difference);
        builder.Ignore(s => s.SetDifference);
    }
}