using System;

using PIED_LMS.Contract.Abstractions.Email;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands;

public class ApproveMentorHandler(UserManager<ApplicationUser> userManager, IEmailService emailService)
    : IRequestHandler<ApproveMentorCommand, ServiceResponse<string>>
{
    public async Task<ServiceResponse<string>> Handle(ApproveMentorCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return new ServiceResponse<string>(false, "User not found");

        user.IsActive = true;
        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            return new ServiceResponse<string>(false, "Failed to approve mentor");

        if (string.IsNullOrEmpty(user.Email))
            return new ServiceResponse<string>(true, "Approved (No email sent: User email is missing)");

        await emailService.SendEmailAsync(user.Email, "Approved", "You can now login.", ct);
        return new ServiceResponse<string>(true, "Approved");
    }

}
