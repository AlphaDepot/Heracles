using Heracles.Api.IntegrationTests.DbTestData;
using Heracles.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Heracles.Api.IntegrationTests;

/// <summary>
///     Factory class for setting up the web application for integration testing.
/// </summary>
public class HeraclesWebApplicationFactory : WebApplicationFactory<Program>
{
	/// <summary>
	///     Container for managing a PostgresSQL database instance for integration testing.
	/// </summary>
	private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder()
		.WithImage("postgres:latest")
		.WithDatabase("heracles")
		.WithUsername("postgres")
		.WithPassword("postgres")
		.Build();

	/// <summary>
	///     Configures the web host builder for the integration testing.
	/// </summary>
	/// <param name="builder">The web host builder.</param>
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");
		builder.ConfigureTestServices(services =>
		{
			// Set up the testing database and authentication.
			services.SetupTestingDatabase(_postgresSqlContainer.GetConnectionString());
			services.SetupTestingAuthentication();

			// ⭐ SEED THE TESTCONTAINERS DB HERE ⭐
			using var scope = services.BuildServiceProvider().CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			context.Database.EnsureDeleted();
			context.Database.Migrate();
			TestDataInitializer.Initialize(context);

		});
	}

	/// <summary>
	///     Initialize method is responsible for starting the Postgres database container for integration testing.
	/// </summary>
	[OneTimeSetUp]
	public async Task InitializeAsync()
	{
		await _postgresSqlContainer.StartAsync();
	}

	/// <summary>
	///     Disposes the resources used by the web application factory.
	/// </summary>
	[OneTimeTearDown]
	public override async ValueTask DisposeAsync()
	{
		await _postgresSqlContainer.DisposeAsync();
	}
}
