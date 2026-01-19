namespace PIED_LMS.Infrastructure.Identity;

/// <summary>
///     Internal interface for setting refresh token cookies
///     Allows AuthService to communicate refresh tokens securely without exposing them in responses
/// </summary>
internal interface IRefreshTokenCookieProvider
{
  void SetRefreshTokenCookie(string refreshToken);
}
