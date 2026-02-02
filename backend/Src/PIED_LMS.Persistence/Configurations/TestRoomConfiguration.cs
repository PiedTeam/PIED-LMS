using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Persistence.Configurations;

public class TestRoomConfiguration : IEntityTypeConfiguration<TestRoom>
{
    public void Configure(EntityTypeBuilder<TestRoom> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.JoinCode)
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.HasIndex(x => x.JoinCode);
    }
}
