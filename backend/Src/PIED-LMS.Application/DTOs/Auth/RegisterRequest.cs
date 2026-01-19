namespace PIED_LMS.Application.DTOs.Auth;

public sealed record RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public string? UserName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
