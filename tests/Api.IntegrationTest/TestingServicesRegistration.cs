using Api.IntegrationTest.Handlers;
using Application.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTest;

/// <summary>
///     Class with helper methods for setting up testing dependencies.
/// </summary>
public static class TestingServicesRegistration
{
	/// <summary>
	///     Sets up the testing database for integration testing.
	/// </summary>
	/// <param name="services">The IServiceCollection instance.</param>
	/// <param name="connectionString">The connection string for the testing database.</param>
	/// <returns>The modified IServiceCollection instance.</returns>
	public static IServiceCollection SetupTestingDatabase(
		this IServiceCollection services,
		string connectionString)
	{
		// Get the existing db context service descriptor
		var descriptor = services.SingleOrDefault(
			d => d.ServiceType ==
			     typeof(DbContextOptions<AppDbContext>));

		// Remove the existing db context service descriptor
		if (descriptor != null)
		{
			services.Remove(descriptor);
		}

		// Add a new database context just for testing
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseNpgsql(connectionString);
			options.EnableSensitiveDataLogging();
		});

		//  Get the service provider for the db context
		var dbContext = services.BuildServiceProvider().GetRequiredService<AppDbContext>();
		// Ensure the database removed of all old data
		dbContext.Database.EnsureDeleted();

		return services;
	}

	/// <summary>
	///     Sets up the testing authentication for integration testing.
	/// </summary>
	/// <param name="services">The IServiceCollection instance.</param>
	/// <returns>The modified IServiceCollection instance.</returns>
	public static IServiceCollection SetupTestingAuthentication(
		this IServiceCollection services)
	{
		// Get the existing authentication service descriptor
		var descriptor = services.SingleOrDefault(
			d => d.ServiceType ==
			     typeof(IAuthenticationService));

		// Remove the existing authentication service descriptor
		if (descriptor != null)
		{
			services.Remove(descriptor);
		}

		// Add a new authentication service just for testing
		services.AddAuthentication("Test")
			.AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

		return services;
	}
}
