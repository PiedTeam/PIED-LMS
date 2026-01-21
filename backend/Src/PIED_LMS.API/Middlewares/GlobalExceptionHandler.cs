using PIED_LMS.Domain.Exceptions;

namespace PIED_LMS.API.Middlewares;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync
    (
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            IdentityException.TokenException => StatusCodes.Status401Unauthorized,
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            Application.Exceptions.ValidationException => StatusCodes.Status422UnprocessableEntity,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = exception is DomainException domainEx ? domainEx.Title : "Server Error",
            Detail = exception.Message,
            Type = exception.GetType().Name
        };

        if (exception is ValidationException valEx) problemDetails.Extensions["errors"] = valEx.Errors;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
