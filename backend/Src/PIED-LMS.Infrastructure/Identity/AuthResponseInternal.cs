namespace PIED_LMS.Infrastructure.Identity;

/// <summary>
///     Internal DTO for auth service to track access and refresh tokens
///     This is internal and not exposed via public APIs to prevent token leakage
/// </summary>
internal sealed record AuthResponseInternal
{
  public required string AccessToken { get; init; }
  public required string RefreshToken { get; init; }
  public required DateTime ExpiresAt { get; init; }
  public required string Email { get; init; }
  public required string UserId { get; init; }
}
