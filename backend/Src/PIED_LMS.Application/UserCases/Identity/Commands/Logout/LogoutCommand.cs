using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Commands.Logout;

public record LogoutCommand(
    Guid UserId,
    string RefreshToken
) : IRequest<ServiceResponse<string>>;
