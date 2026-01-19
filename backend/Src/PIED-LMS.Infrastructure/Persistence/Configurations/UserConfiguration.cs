using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("domain_users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Ignore(u => u.FullName);
    }
}
