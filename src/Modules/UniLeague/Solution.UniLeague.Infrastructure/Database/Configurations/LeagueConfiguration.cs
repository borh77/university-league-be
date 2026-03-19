using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Solution.UniLeague.Core.Domain;

namespace Solution.UniLeague.Infrastructure.Database.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        builder.ToTable("Leagues");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Sport)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.LeagueGender)
            .HasConversion<string>()
            .IsRequired(false);

       
        builder.Navigation(l => l.Standings)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(l => l.Standings)
            .WithOne()
            .HasForeignKey(s => s.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}