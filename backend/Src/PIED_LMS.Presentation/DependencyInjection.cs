using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using PIED_LMS.Presentation.Mappings;

namespace PIED_LMS.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        // Register Mapster
        var config = MappingConfig.GetTypeAdapterConfig();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
