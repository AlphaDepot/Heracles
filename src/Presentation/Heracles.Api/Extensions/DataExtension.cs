using Heracles.Persistence;
using Heracles.Persistence.SeedData;
using Heracles.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Api.Extensions;

public static class DataExtension
{
	public static IApplicationBuilder LoadTestingSeedData(this IApplicationBuilder app)
	{
		// Check if the app is null and throw an ArgumentNullException if it is
		ArgumentNullException.ThrowIfNull(app);

		// Create a scope to get the service provider
		using var scope = app.ApplicationServices.CreateScope();
		// Get the service provider
		var services = scope.ServiceProvider;


		// Try to seed the database
		try
		{
			// Get the HeraclesDbContext
			var context = services.GetRequiredService<AppDbContext>();

			context.Database.Migrate();
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
