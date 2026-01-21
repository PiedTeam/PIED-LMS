using PIED_LMS.Application.Abstractions;
using PIED_LMS.Contract.Services.Identity;
using PIED_LMS.Domain.Entities;

namespace PIED_LMS.Application.UserCases.Commands;

public class RefreshTokenCommandHandler(
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService,
    UserManager<ApplicationUser> userManager
)
    : IRequestHandler<RefreshTokenCommand, ServiceResponse<RefreshTokenResponse>>
{
    public async Task<ServiceResponse<RefreshTokenResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate refresh token
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return new ServiceResponse<RefreshTokenResponse>(false, "Invalid refresh token");

            // Get user ID from refresh token
            var userId = await refreshTokenService.GetUserIdFromRefreshTokenAsync(request.RefreshToken);
            if (userId is null)
                return new ServiceResponse<RefreshTokenResponse>(false, "Invalid or expired refresh token");

            // Get user and validate
            var user = await userManager.FindByIdAsync(userId.Value.ToString());
            if (user is null || !user.IsActive)
                return new ServiceResponse<RefreshTokenResponse>(false, "User not found or inactive");

            // Get user roles and claims
            var userRoles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("FirstName", user.FirstName),
                new("LastName", user.LastName)
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Generate new access token
            var newAccessToken = jwtTokenService.GenerateAccessToken(claims);

            var response = new RefreshTokenResponse(newAccessToken);
            return new ServiceResponse<RefreshTokenResponse>(true, "Token refreshed successfully", response);
        }
        catch (Exception ex)
        {
            return new ServiceResponse<RefreshTokenResponse>(
                false,
                $"Refresh token failed: {ex.Message}"
            );
        }
    }
}
