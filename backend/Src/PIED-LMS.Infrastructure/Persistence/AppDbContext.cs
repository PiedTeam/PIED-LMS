using PIED_LMS.Domain.Entities;
using PIED_LMS.Infrastructure.Identity;

namespace PIED_LMS.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<UserProfile, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<User> DomainUsers { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
