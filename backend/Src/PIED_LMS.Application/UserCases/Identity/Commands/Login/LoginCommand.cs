namespace PIED_LMS.Application.UserCases.Identity.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ServiceResponse<LoginResult>>;

public record LoginResult(LoginResponse Response, string RefreshToken);
