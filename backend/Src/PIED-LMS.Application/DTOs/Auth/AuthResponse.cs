namespace PIED_LMS.Application.DTOs.Auth;

/// <summary>
///     Authentication response containing access token and user info.
///     Refresh token is sent as a secure HttpOnly cookie via Set-Cookie header
/// </summary>
public sealed record AuthResponse
{
    public required string AccessToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string Email { get; init; }
    public required string UserId { get; init; }
}

/// <summary>
///     Internal DTO for auth service to track refresh token before setting cookie
/// </summary>
public sealed record AuthResponseInternal
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string Email { get; init; }
    public required string UserId { get; init; }
}
