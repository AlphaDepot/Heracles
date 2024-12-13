using Application.Infrastructure.Data;
using Application.Infrastructure.Data.SeedData;
using Application.Infrastructure.Exceptions;
using Application.Infrastructure.Logging;
using Application.Infrastructure.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Application;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		// Setup Database
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});

		// Global Exception Handling
		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddProblemDetails();


		// Configure HttpContextAccessor
		services.AddHttpContextAccessor();

		// Configure AppLogger
		services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

		// Setup MediatR
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblies(typeof(ApplicationAssemblyReference).Assembly);
			cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
			cfg.AddOpenBehavior(typeof(FluentValidationBehavior<,>));
		});

		// Setup FluentValidation
		//services.AddValidatorsFromAssemblyContaining(typeof(ApplicationAssemblyReference), includeInternalTypes: true);

		return services;
	}

	public static IHostBuilder UseApplicationSerilog(this IHostBuilder hostBuilder)
	{
		hostBuilder.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
		return hostBuilder;
	}

	public static IApplicationBuilder LoadTestingSeedData(this IApplicationBuilder app)
	{
		// Check if the app is null and throw an ArgumentNullException if it is
		ArgumentNullException.ThrowIfNull(app, nameof(app));

		// Create a scope to get the service provider
		using var scope = app.ApplicationServices.CreateScope();
		// Get the service provider
		var services = scope.ServiceProvider;


		// Try to seed the database
		try
		{
			// Get the HeraclesDbContext
			var context = services.GetRequiredService<AppDbContext>();
			DataInitializer.Initialize(context);
		}
		catch (Exception ex)
		{
			// var logger = services.GetRequiredService<ILogger<Program>>();
			var logger = services.GetRequiredService<IAppLogger<DataInitializer>>();
			logger.LogError($"An error occurred while seeding the database. {ex}", ex);
		}

		return app;
	}
}
