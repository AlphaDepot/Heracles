using Heracles.Application.Logging;
using Heracles.Application.Validation;
using Heracles.Shared.Interfaces;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Application;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		// Configure HttpContextAccessor
		//services.AddHttpContextAccessor();

		// Configure AppLogger
		services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));


		// Mediator
		services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });

		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
		services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));


		return services;
	}
}
