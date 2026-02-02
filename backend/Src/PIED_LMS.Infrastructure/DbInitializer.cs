using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using PIED_LMS.Domain.Constants;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Infrastructure;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        string[] roles = { RoleConstants.Administrator, RoleConstants.Teacher, RoleConstants.Student, RoleConstants.Mentor };

        // 1. Seed Roles
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            }
        }

        // 2. Seed Users (Environment Gated)
        // Only seed default users in Development OR if passwords are explicitly configured
        
        var adminPassword = configuration["Seed:AdminPassword"];
        var shouldSeedAdmin = !string.IsNullOrEmpty(adminPassword) || env.IsDevelopment();
        if (string.IsNullOrEmpty(adminPassword) && env.IsDevelopment()) adminPassword = "Admin@123";

        if (shouldSeedAdmin && !string.IsNullOrEmpty(adminPassword))
        {
            var adminEmail = "admin@pied.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RoleConstants.Administrator);
                }
            }
        }

        var teacherPassword = configuration["Seed:TeacherPassword"];
        var shouldSeedTeacher = !string.IsNullOrEmpty(teacherPassword) || env.IsDevelopment();
        if (string.IsNullOrEmpty(teacherPassword) && env.IsDevelopment()) teacherPassword = "Teacher@123";

        if (shouldSeedTeacher && !string.IsNullOrEmpty(teacherPassword))
        {
            var teacherEmail = "teacher@pied.com";
            var teacherUser = await userManager.FindByEmailAsync(teacherEmail);
            if (teacherUser == null)
            {
                teacherUser = new ApplicationUser
                {
                    UserName = teacherEmail,
                    Email = teacherEmail,
                    FirstName = "John",
                    LastName = "Teacher",
                    IsActive = true,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(teacherUser, teacherPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacherUser, RoleConstants.Teacher);
                }
            }
        }
    }
}
