namespace PIED_LMS.API.Filters;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    private static readonly HashSet<string> _publicEndpoints =
    [
        "/api/auth/login",
        "/api/auth/register"
    ];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Get endpoint path
        var path = context.ApiDescription.RelativePath;

        // Remove security requirement for public endpoints
        if (_publicEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase)) operation.Security = null;
    }
}
