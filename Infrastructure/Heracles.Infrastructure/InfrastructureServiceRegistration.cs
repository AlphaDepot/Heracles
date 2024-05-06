using Heracles.Domain.Abstractions.Logging;
using Heracles.Infrastructure.Exceptions;
using Heracles.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddMemoryCache();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        

        return services;
    }
}