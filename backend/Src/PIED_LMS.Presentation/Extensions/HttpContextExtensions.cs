using System.Security.Claims;

namespace PIED_LMS.Presentation.Extensions;

public static class HttpContextExtensions
{
    public static string GetIdentifier(this HttpContext context)
    {
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId)) return $"user:{userId}";

        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                 ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                 ?? context.Connection.RemoteIpAddress?.ToString()
                 ?? "unknown";

        return $"ip:{ip}";
    }
}
