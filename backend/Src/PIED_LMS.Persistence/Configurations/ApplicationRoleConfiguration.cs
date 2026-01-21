using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("roles");

        builder.Property(r => r.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Seed default roles with deterministic values
        var adminRoleId = Guid.Parse("b4a5b0c5-9c3a-4f0a-8e1f-6d2c3a1b2f10");
        var mentorRoleId = Guid.Parse("7a2f3c45-1d6b-4e8f-9a0b-3c2d1e4f5a67");
        var studentRoleId = Guid.Parse("3c1d2e4f-5a67-4b8f-9a0b-7a2f3c451d6b");
        var seededAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new ApplicationRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with full access",
                CreatedAt = seededAtUtc,
                ConcurrencyStamp = "role-admin-concurrency"
            },
            new ApplicationRole
            {
                Id = mentorRoleId,
                Name = "Mentor",
                NormalizedName = "MENTOR",
                Description = "Mentor who can create and manage courses",
                CreatedAt = seededAtUtc,
                ConcurrencyStamp = "role-mentor-concurrency"
            },
            new ApplicationRole
            {
                Id = studentRoleId,
                Name = "Student",
                NormalizedName = "STUDENT",
                Description = "Student who can enroll in courses",
                CreatedAt = seededAtUtc,
                ConcurrencyStamp = "role-student-concurrency"
            }
        );
    }
}
