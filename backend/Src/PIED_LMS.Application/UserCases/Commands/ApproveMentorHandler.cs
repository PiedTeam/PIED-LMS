using System;
using System.Collections.Generic;
using System.Text;
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
        user.IsActive = true;
        await userManager.UpdateAsync(user);

        await emailService.SendEmailAsync(user.Email, "Approved", "You can now login.");
        return new ServiceResponse<string>(true, "Approved");
    }

}
