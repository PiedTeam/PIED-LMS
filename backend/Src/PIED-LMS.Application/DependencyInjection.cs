using Microsoft.Extensions.DependencyInjection;

namespace PIED_LMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>(ServiceLifetime.Singleton);

        return services;
    }
}

// Marker class for assembly scanning
file sealed class ApplicationAssemblyMarker;
