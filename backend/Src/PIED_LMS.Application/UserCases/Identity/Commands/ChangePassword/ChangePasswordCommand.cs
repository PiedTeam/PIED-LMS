using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<ServiceResponse<string>>;
