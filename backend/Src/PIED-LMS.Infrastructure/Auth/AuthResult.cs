using PIED_LMS.Application.DTOs.Auth;

namespace PIED_LMS.Infrastructure.Auth;

/// <summary>
///     Internal wrapper that combines the public auth response with the refresh token
///     This is only used internally to pass the refresh token from service to controller
/// </summary>
internal sealed record AuthResult
{
  public required AuthResponse PublicResponse { get; init; }
  public required string RefreshToken { get; init; }
}
