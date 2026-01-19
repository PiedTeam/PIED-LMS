using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Domain.Common;

namespace PIED_LMS.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    // Overload that accepts access token and refresh token directly (for cookie-based flows)
    Task<Result<AuthResponse>> RefreshTokenAsync(string accessToken, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(string userId, CancellationToken cancellationToken = default);
    Task<string?> GetLatestRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}
