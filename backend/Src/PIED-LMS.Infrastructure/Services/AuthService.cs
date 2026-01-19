using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Application.Services;
using PIED_LMS.Domain.Common;
using PIED_LMS.Domain.Entities;
using PIED_LMS.Infrastructure.Identity;
using PIED_LMS.Infrastructure.Options;
using PIED_LMS.Infrastructure.Persistence;

namespace PIED_LMS.Infrastructure.Services;

public sealed class AuthService(
    UserManager<UserProfile> userManager,
    SignInManager<UserProfile> signInManager,
    IOptions<JwtOptions> jwtOptions,
    AppDbContext dbContext
) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Password != request.ConfirmPassword)
            return Result<AuthResponse>.Failure("AUTH_PASSWORD_MISMATCH", "Passwords do not match");

        // Create domain user entity for business logic
        var domainUser = new User(
            Guid.CreateVersion7(),
            request.Email,
            request.FirstName,
            request.LastName
        );

        // Save domain user first (required for FK constraint)
        dbContext.DomainUsers.Add(domainUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Create infrastructure user for authentication with reference to domain user
        var appUser = new UserProfile
        {
            Email = request.Email,
            UserName = request.UserName ?? request.Email,
            EmailConfirmed = false,
            DomainUserId = domainUser.Id
        };

        var result = await userManager.CreateAsync(appUser, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure("AUTH_REGISTRATION_FAILED", errors);
        }

        var authResponseInternal = await GenerateAuthResponseAsync(appUser, cancellationToken);
        var authResponse = new AuthResponse
        {
            AccessToken = authResponseInternal.AccessToken,
            ExpiresAt = authResponseInternal.ExpiresAt,
            Email = authResponseInternal.Email,
            UserId = authResponseInternal.UserId
        };
        return Result<AuthResponse>.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Result<AuthResponse>.Failure("AUTH_INVALID_CREDENTIALS", "Invalid email or password");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (!result.Succeeded)
            return result.IsLockedOut
                ? Result<AuthResponse>.Failure("AUTH_ACCOUNT_LOCKED", "Account is locked out")
                : Result<AuthResponse>.Failure("AUTH_INVALID_CREDENTIALS", "Invalid email or password");

        var authResponseInternal = await GenerateAuthResponseAsync(user, cancellationToken);
        var authResponse = new AuthResponse
        {
            AccessToken = authResponseInternal.AccessToken,
            ExpiresAt = authResponseInternal.ExpiresAt,
            Email = authResponseInternal.Email,
            UserId = authResponseInternal.UserId
        };
        return Result<AuthResponse>.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);

        if (principal == null)
            return Result<AuthResponse>.Failure("AUTH_INVALID_TOKEN", "Invalid access token");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return Result<AuthResponse>.Failure("AUTH_INVALID_CLAIMS", "Invalid token claims");

        var refreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userGuid, cancellationToken);

        if (refreshToken is not { IsActive: true })
            return Result<AuthResponse>.Failure("AUTH_INVALID_REFRESH_TOKEN", "Invalid or expired refresh token");

        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
            return Result<AuthResponse>.Failure("AUTH_USER_NOT_FOUND", "User not found");

        // Revoke old refresh token
        refreshToken.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var authResponseInternal = await GenerateAuthResponseAsync(user, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var authResponse = new AuthResponse
        {
            AccessToken = authResponseInternal.AccessToken,
            ExpiresAt = authResponseInternal.ExpiresAt,
            Email = authResponseInternal.Email,
            UserId = authResponseInternal.UserId
        };
        return Result<AuthResponse>.Success(authResponse);
    }

    public async Task<Result> LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            return Result.Failure("AUTH_INVALID_USER_ID", "Invalid user ID");

        var refreshTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userGuid && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens) token.RevokedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<string?> GetLatestRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var refreshToken = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .OrderByDescending(rt => rt.CreatedAt)
            .Select(rt => rt.Token)
            .FirstOrDefaultAsync(cancellationToken);

        return refreshToken;
    }

    private async Task<AuthResponseInternal> GenerateAuthResponseAsync(UserProfile user,
        CancellationToken cancellationToken)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseInternal
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            Email = user.Email!,
            UserId = user.Id.ToString()
        };
    }

    private string GenerateAccessToken(UserProfile user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Don't validate lifetime for refresh
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidAudience = _jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
