using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Data;

public sealed class IdpDbContext(DbContextOptions<IdpDbContext> options) : DbContext(options)
{
    public DbSet<RegisteredUser> Users => Set<RegisteredUser>();
}
