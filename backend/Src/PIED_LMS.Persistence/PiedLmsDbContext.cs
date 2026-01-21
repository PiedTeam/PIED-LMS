using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Persistence;

public class PiedLmsDbContext(DbContextOptions<PiedLmsDbContext> options) : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable PostgreSQL extension for UUIDv7
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PiedLmsDbContext).Assembly);
    }
}
