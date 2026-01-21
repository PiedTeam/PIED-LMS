namespace PIED_LMS.Application.Abstractions;

public interface IAuthenticationService
{
    Task<(bool Success, string Message)>
        RegisterAsync(string email, string firstName, string lastName, string password);

    Task<(bool Success, string Message, string? AccessToken, string? RefreshToken)> LoginAsync(string email,
        string password);

    Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<(bool Success, string Message)> LogoutAsync(Guid userId);
    Task<(bool Success, string Message)> AssignRoleAsync(Guid userId, string roleName);
}
