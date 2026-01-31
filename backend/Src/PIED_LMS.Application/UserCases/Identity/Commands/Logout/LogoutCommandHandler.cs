namespace PIED_LMS.Application.UserCases.Identity.Commands.Logout;

public class LogoutCommandHandler(
    IRefreshTokenService refreshTokenService,
    ILogger<LogoutCommandHandler> logger)
    : IRequestHandler<LogoutCommand, ServiceResponse<string>>
{
  public async Task<ServiceResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
  {
    try
    {
      if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        await refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);

      return new ServiceResponse<string>(true, "Logout successful", "User logged out");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Logout failed for user {UserId}", request.UserId);
      return new ServiceResponse<string>(false, "Logout failed");
    }
  }
}
