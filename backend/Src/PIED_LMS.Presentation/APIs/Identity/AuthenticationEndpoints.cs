namespace PIED_LMS.Presentation.APIs.Identity;

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
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .Produces<ServiceResponse<UserResponse>>()
            .Produces<ServiceResponse<UserResponse>>(StatusCodes.Status404NotFound);

        group.MapGet("/users", GetAllUsers)
            .WithName("GetAllUsers")
            .WithOpenApi()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrator" })
            .Produces<ServiceResponse<PaginatedResponse<UserResponse>>>();
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        IMapper mapper,
        IMediator mediator)
    {
        var command = mapper.Map<RegisterCommand>(request);
        var result = await mediator.Send(command);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        IMapper mapper,
        IMediator mediator,
        HttpContext context,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var command = mapper.Map<LoginCommand>(request);
        var result = await mediator.Send(command);

        if (!result.Success || result.Data == null)
            return Results.Unauthorized();

        // Extract login result (contains response and refresh token)
        var loginResult = result.Data;

        // Set refresh token in HttpOnly cookie
        var refreshTokenExpirationDays = configuration.GetValue("JwtSettings:RefreshTokenExpirationDays", 7);
        var secureCookie = configuration.GetValue("Cookies:Secure", !environment.IsDevelopment());
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = secureCookie,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            Path = "/api/auth/refresh"
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
        IMediator mediator,
        ILogger<AuthenticationEndpoints> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Get refresh token from cookie
        var refreshToken = context.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            logger.LogWarning("Refresh token missing in cookie. HasRefreshTokenCookie: {HasCookie}",
                context.Request.Cookies.ContainsKey("refreshToken"));
            return Results.Json(new { error = "Invalid refresh token" }, statusCode: 401);
        }

        var command = new RefreshTokenCommand(refreshToken);
        var result = await mediator.Send(command);

        if (!result.Success || result.Data == null)
        {
            logger.LogWarning("Refresh token request failed: {Message}", result.Message);
            return Results.Json(new { error = "Invalid refresh token" }, statusCode: 401);
        }

        // Update refresh token cookie
        var refreshTokenExpirationDays = configuration.GetValue("JwtSettings:RefreshTokenExpirationDays", 7);
        var secureCookie = configuration.GetValue("Cookies:Secure", !environment.IsDevelopment());
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = secureCookie,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
            Path = "/api/auth/refresh"
        };

        context.Response.Cookies.Append("refreshToken", result.Data.RefreshToken, cookieOptions);

        // Return new access token
        return Results.Ok(result);
    }

    private static async Task<IResult> Logout(
        HttpContext context,
        IMediator mediator)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        // Get refresh token from cookie and revoke it
        var refreshToken = context.Request.Cookies["refreshToken"];

        // Delete refresh token cookie
        context.Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/auth/refresh" });

        var command = new LogoutCommand(userId, refreshToken ?? string.Empty);
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

        // ChangePassword requires userId from context, so manual mapping is needed
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
        IMapper mapper,
        IMediator mediator)
    {
        var command = mapper.Map<AssignRoleCommand>(request);
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
        IMapper mapper,
        IMediator mediator)
    {
        var query = mapper.Map<GetAllUsersQuery>(request);
        var result = await mediator.Send(query);
        return result.Success ? Results.Ok(result) : Results.BadRequest(result);
    }
}
