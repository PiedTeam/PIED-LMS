using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using PIED_LMS.Domain.Entities;
using PIED_LMS.Infrastructure.Identity;

namespace PIED_LMS.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<User> DomainUsers { get; set; }
}
