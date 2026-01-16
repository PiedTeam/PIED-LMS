using PIED_LMS.Domain.Entities;
using PIED_LMS.Infrastructure.Identity;

namespace PIED_LMS.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("accounts");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
