using System;
using System.Collections.Generic;
using System.Text;
using PIED_LMS.Contract.Abstractions.Email;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands;

public class ImportStudentsHandler(UserManager<ApplicationUser> userManager, IEmailService emailService) 
    : IRequestHandler<ImportStudentsCommand, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(ImportStudentsCommand request, CancellationToken ct)
    {
        foreach (var st in request.Students) {
            var password = Guid.NewGuid().ToString().Substring(0,8) + "A@1";
            var user = new ApplicationUser { UserName = st.Email, Email = st.Email, IsActive = true };
            
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded) {
                await userManager.AddToRoleAsync(user, "Student");
                await emailService.SendEmailAsync(st.Email, "Account Created", $"Pass: {password}");
            }
        }
        return new ServiceResponse<string>(true, "Imported");
    }
}
