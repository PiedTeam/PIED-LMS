using System.Security.Claims;
using PIED_LMS.Application.Abstractions;
using PIED_LMS.Contract.Services.Identity;

namespace PIED_LMS.Presentation.APIs;

public class AuthenticationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithName("Authentication")
            .WithOpenApi();

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithOpenApi()
            .Produces<ServiceResponse<RegisterResponse>>()
            .Produces<ServiceResponse<RegisterResponse>>(StatusCodes.Status400BadRequest);

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithOpenApi()
            .Produces<ServiceResponse<LoginResponse>>()
            .Produces<ServiceResponse<LoginResponse>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithOpenApi()
            .Produces<ServiceResponse<RefreshTokenResponse>>()
            .Produces<ServiceResponse<RefreshTokenResponse>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<ServiceResponse<string>>()
            .Produces<ServiceResponse<string>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<ServiceResponse<string>>()
            .Produces<ServiceResponse<string>>(StatusCodes.Status400BadRequest);

        group.MapPost("/assign-role", AssignRole)
            .WithName("AssignRole")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<ServiceResponse<string>>()
            .Produces<ServiceResponse<string>>(StatusCodes.Status400BadRequest);

        group.MapGet("/users/{id}", GetUserById)
            .WithName("GetUserById")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<ServiceResponse<UserResponse>>()
            .Produces<ServiceResponse<UserResponse>>(StatusCodes.Status404NotFound);

        group.MapGet("/users", GetAllUsers)
            .WithName("GetAllUsers")
            .WithOpenApi()
            .RequireAuthorization()
            .Produces<ServiceResponse<PaginatedResponse<UserResponse>>>();
    }

    private static async Task<IResult> Register(
        RegisterCommand request,
        IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> Login(
        LoginCommand request,
        IMediator mediator,
        HttpContext context,
        IConfiguration configuration)
    {
        var result = await mediator.Send(request);

        if (!result.Success || result.Data == null)
            return Results.Unauthorized();

        // Extract login result (contains response and refresh token)
        var loginResult = result.Data;

        // Set refresh token in HttpOnly cookie
        var refreshTokenExpirationDays = configuration.GetValue("JwtSettings:RefreshTokenExpirationDays", 7);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Set to false for development (localhost), set to true in production
            SameSite = SameSiteMode.Lax, // Changed from Strict to Lax for better compatibility
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            Path = "/"
        };

        context.Response.Cookies.Append("refreshToken", loginResult.RefreshToken, cookieOptions);

        // Return only login response (without refresh token)
        var loginResponse = new ServiceResponse<LoginResponse>(
            result.Success,
            result.Message,
            loginResult.Response
        );

        return Results.Ok(loginResponse);
    }

    private static async Task<IResult> RefreshToken(
        HttpContext context,
        IMediator mediator)
    {
        // Get refresh token from cookie
        var refreshToken = context.Request.Cookies["refreshToken"];

        // Debug: Log all cookies
        var allCookies = string.Join(", ",
            context.Request.Cookies.Select(c =>
                $"{c.Key}={c.Value?.Substring(0, Math.Min(20, c.Value?.Length ?? 0))}..."));

        if (string.IsNullOrEmpty(refreshToken))
            // Return more detailed error
            return Results.Json(new { error = "Refresh token not found in cookie", cookies = allCookies },
                statusCode: 401);

        var command = new RefreshTokenCommand(refreshToken);
        var result = await mediator.Send(command);

        if (!result.Success || result.Data == null)
            // Return error message for debugging
            return Results.Json(new { error = result.Message ?? "Invalid refresh token" }, statusCode: 401);

        // Return new access token
        return Results.Ok(result);
    }

    private static async Task<IResult> Logout(
        HttpContext context,
        IMediator mediator,
        IRefreshTokenService refreshTokenService)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        // Get refresh token from cookie and revoke it
        var refreshToken = context.Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken)) await refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        // Delete refresh token cookie
        context.Response.Cookies.Delete("refreshToken");

        var command = new LogoutCommand(userId);
        var result = await mediator.Send(command);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> ChangePassword(
        HttpContext context,
        ChangePasswordRequest request,
        IMediator mediator)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmPassword
        );

        var result = await mediator.Send(command);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> AssignRole(
        AssignRoleRequest request,
        IMediator mediator)
    {
        var command = new AssignRoleCommand(request.UserId, request.RoleName);
        var result = await mediator.Send(command);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        IMediator mediator)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query);
        return result.Success ? Results.Ok(result) : Results.NotFound(result);
    }

    private static async Task<IResult> GetAllUsers(
        [AsParameters] GetAllUsersRequest request,
        IMediator mediator)
    {
        var query = new GetAllUsersQuery(request.PageNumber, request.PageSize);
        var result = await mediator.Send(query);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public record AssignRoleRequest(
    Guid UserId,
    string RoleName
);

public record GetAllUsersRequest(
    int PageNumber = 1,
    int PageSize = 10
);
