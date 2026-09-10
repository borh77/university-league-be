using Microsoft.EntityFrameworkCore;
using Solution.Identity.Core.Domain;
using Solution.Identity.Infrastructure.Database.Configurations;

namespace Solution.Identity.Infrastructure.Database;

public class IdentityContext : DbContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
