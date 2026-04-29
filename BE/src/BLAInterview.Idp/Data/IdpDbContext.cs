using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Data;

public sealed class IdpDbContext(DbContextOptions<IdpDbContext> options) : DbContext(options)
{
    public DbSet<RegisteredUser> Users => Set<RegisteredUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<RegisteredUser>();

        user.HasKey(registeredUser => registeredUser.Id);

        user.Property(registeredUser => registeredUser.Id)
            .ValueGeneratedOnAdd();

        user.HasIndex(registeredUser => registeredUser.NormalizedEmail)
            .IsUnique();
    }
}
