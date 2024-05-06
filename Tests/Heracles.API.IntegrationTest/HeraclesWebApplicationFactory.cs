using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Testcontainers.PostgreSql;


namespace Heracles.API.IntegrationTest;

/// <summary>
/// Factory class for setting up the web application for integration testing.
/// </summary>
public class HeraclesWebApplicationFactory : WebApplicationFactory<Program> , IAsyncLifetime
{
    /// <summary>
    /// Container for managing a PostgreSQL database instance for integration testing.
    /// </summary>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("heracles")
        .WithUsername("postgres")
        .WithPassword( "postgres")
        .Build();


    /// <summary>
    /// Configures the web host builder for the integration testing.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Setup the testing database and authentication.
            services.SetupTestingDatabase(_postgreSqlContainer.GetConnectionString());
            services.SetupTestingAuthentication();
        });
    }

    /// <summary>
    /// InitializeAsync method is responsible for starting the Postgres database container for integration testing.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    /// <summary>
    /// Asynchronously disposes the resources used by the web application factory.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}