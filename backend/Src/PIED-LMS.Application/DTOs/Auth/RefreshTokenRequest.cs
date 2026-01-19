namespace PIED_LMS.Application.DTOs.Auth;

public sealed record RefreshTokenRequest
{
    public required string AccessToken { get; init; }
}
