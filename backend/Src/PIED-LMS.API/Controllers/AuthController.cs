using PIED_LMS.Application.DTOs.Auth;
using PIED_LMS.Application.Services;
using PIED_LMS.Domain.Common;

namespace PIED_LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger, IWebHostEnvironment env)
    : ControllerBase
{
    private const string _unknownErrorMessage = "Unknown error";
    private const string _unknownErrorCode = "UNKNOWN_ERROR";
    private const string _refreshTokenCookieName = "refresh_token";
    private const int _refreshTokenCookieMaxAge = 7 * 24 * 60 * 60; // 7 days in seconds

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

        if (Guid.TryParse(result.Data!.UserId, out var userId))
        {
            logger.LogInformation("Retrieving refresh token for userId: {UserId}", userId);
            var refreshToken = await authService.GetLatestRefreshTokenAsync(userId, cancellationToken);
            if (refreshToken != null)
            {
                logger.LogInformation("Successfully retrieved refresh token, setting cookie");
                SetRefreshTokenCookie(refreshToken);
            }
            else
            {
                logger.LogWarning("No refresh token found for userId: {UserId}", userId);
            }
        }
        else
        {
            logger.LogWarning("Failed to parse UserId: {UserId}", result.Data!.UserId);
        }

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

        if (!Guid.TryParse(result.Data!.UserId, out var userId)) return Ok(result);
        var refreshToken = await authService.GetLatestRefreshTokenAsync(userId, cancellationToken);
        if (refreshToken is not null)
            SetRefreshTokenCookie(refreshToken);

        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Result<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request, cancellationToken);

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

        if (!Guid.TryParse(result.Data!.UserId, out var userId)) return Ok(result);
        var refreshToken = await authService.GetLatestRefreshTokenAsync(userId, cancellationToken);
        if (refreshToken != null)
            SetRefreshTokenCookie(refreshToken);

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

        Response.Cookies.Delete(_refreshTokenCookieName);

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

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !env.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/api/auth/refresh",
            MaxAge = TimeSpan.FromSeconds(_refreshTokenCookieMaxAge)
        };

        logger.LogInformation("Setting refresh token cookie. Secure={Secure}, Environment={Environment}",
            cookieOptions.Secure, env.EnvironmentName);

        Response.Cookies.Append(_refreshTokenCookieName, refreshToken, cookieOptions);
    }
}
