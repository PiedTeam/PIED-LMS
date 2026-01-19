using PIED_LMS.Infrastructure.Identity;

namespace PIED_LMS.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.DomainUserId)
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(a => a.Email)
            .IsUnique();

        builder.HasIndex(a => a.UserName)
            .IsUnique();
    }
}
