using PIED_LMS.Contract.Services.Identity.Responses;

namespace PIED_LMS.Application.UserCases.Identity.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ServiceResponse<RefreshTokenResponse>>;
