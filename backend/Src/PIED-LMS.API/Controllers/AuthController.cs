using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Application.Services;
using PIED_LMS.Domain.Common;
using PIED_LMS.Infrastructure.Options;

using Microsoft.Extensions.Options;

namespace PIED_LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger,
    IWebHostEnvironment env,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private const string _unknownErrorMessage = "Unknown error";
    private const string _unknownErrorCode = "UNKNOWN_ERROR";
    private const string _refreshTokenCookieName = "refresh_token";
    private const string _refreshTokenCookiePath = "/api/auth/refresh";
    private readonly int _refreshTokenCookieMaxAge = jwtOptions.Value.RefreshTokenExpiryDays * 24 * 60 * 60;

    /// <summary>
    ///     Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Registration failed",
                Detail = result.Error?.Description ?? _unknownErrorMessage,
                Extensions = new Dictionary<string, object?>
                {
                    { "code", result.Error?.Code ?? _unknownErrorCode }
                }
            });

        logger.LogInformation("User registered successfully, setting refresh token cookie");
        TrySetRefreshTokenCookie();

        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Login failed",
                Detail = result.Error?.Description ?? _unknownErrorMessage,
                Extensions = new Dictionary<string, object?>
                {
                    { "code", result.Error?.Code ?? _unknownErrorCode }
                }
            });

        logger.LogInformation("User registered successfully, setting refresh token cookie");
        TrySetRefreshTokenCookie();

        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        // Read refresh token from secure HttpOnly cookie
        if (!Request.Cookies.TryGetValue(_refreshTokenCookieName, out var refreshToken) ||
            string.IsNullOrEmpty(refreshToken))
        {
            logger.LogWarning("Refresh token not found in cookie");
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Token refresh failed",
                Detail = "Refresh token not found in secure cookie",
                Extensions = new Dictionary<string, object?>
                {
                    { "code", "AUTH_MISSING_REFRESH_TOKEN" }
                }
            });
        }

        var result = await authService.RefreshTokenAsync(request.AccessToken, refreshToken, cancellationToken);

        if (!result.IsSuccess)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Token refresh failed",
                Detail = result.Error?.Description ?? _unknownErrorMessage,
                Extensions = new Dictionary<string, object?>
                {
                    { "code", result.Error?.Code ?? _unknownErrorCode }
                }
            });

        logger.LogInformation("Token refreshed successfully, updating refresh token cookie");
        TrySetRefreshTokenCookie();

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Logout failed",
                Detail = "User not authenticated"
            });

        var result = await authService.LogoutAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Logout failed",
                Detail = result.Error?.Description ?? _unknownErrorMessage,
                Extensions = new Dictionary<string, object?>
                {
                    { "code", result.Error?.Code ?? _unknownErrorCode }
                }
            });

        // Delete refresh token cookie with matching path and options
        Response.Cookies.Delete(_refreshTokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = _refreshTokenCookiePath
        });

        logger.LogInformation("Refresh token cookie deleted");

        return Ok(Result.Success());
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userInfo = new
        {
            userId,
            email,
            userName
        };

        return Ok(Result<object>.Success(userInfo));
    }

    // Removed: SetRefreshTokenCookie(AuthResponse) since refresh token is handled via HttpContext.Items

    internal void SetRefreshTokenCookieInternal(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = _refreshTokenCookiePath,
            MaxAge = TimeSpan.FromSeconds(_refreshTokenCookieMaxAge)
        };

        logger.LogInformation("Setting refresh token cookie. Secure={Secure}, Environment={Environment}",
            cookieOptions.Secure, env.EnvironmentName);

        Response.Cookies.Append(_refreshTokenCookieName, refreshToken, cookieOptions);
    }

    private void TrySetRefreshTokenCookie()
    {
        // AuthService stores the refresh token in HttpContext.Items under this key
        const string contextKey = "__internal_refresh_token";
        if (HttpContext.Items.TryGetValue(contextKey, out var value) && value is string token && !string.IsNullOrEmpty(token))
        {
            SetRefreshTokenCookieInternal(token);
        }
        else
        {
            logger.LogWarning("No refresh token found in HttpContext.Items");
        }
    }
}
