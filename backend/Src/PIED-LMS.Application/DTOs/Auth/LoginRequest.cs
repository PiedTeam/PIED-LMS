namespace PIED_LMS.Application.DTOs.Auth;

public sealed record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
