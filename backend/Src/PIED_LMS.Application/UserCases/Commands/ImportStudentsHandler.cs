using System;

using PIED_LMS.Contract.Abstractions.Email;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands;

public class ImportStudentsHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) 
    : IRequestHandler<ImportStudentsCommand, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(ImportStudentsCommand request, CancellationToken ct)
    {
        var errors = new List<string>();
        var successCount = 0;

        foreach (var st in request.Students) {
            // Use RandomNumberGenerator for secure password
            var randomBytes = new byte[10];
            System.Security.Cryptography.RandomNumberGenerator.Fill(randomBytes);
            var password = Convert.ToBase64String(randomBytes).TrimEnd('=') + "A@1";
            var user = new ApplicationUser 
            { 
                UserName = st.Email, 
                Email = st.Email, 
                FirstName = st.FirstName, 
                LastName = st.LastName, 
                IsActive = true 
            };
            
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded) {
                await userManager.AddToRoleAsync(user, "Student");
                await emailService.SendEmailAsync(st.Email, "Account Created", $"Pass: {password}");
                successCount++;
            }
            else
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                errors.Add($"{st.Email}: {errorMsg}");
            }
        }

        if (errors.Count > 0)
        {
             return new ServiceResponse<string>(false, $"Imported {successCount}/{request.Students.Count}. Failures: {string.Join(" | ", errors)}");
        }

        return new ServiceResponse<string>(true, $"Successfully imported all {successCount} students.");
    }
}
