using System.Net.Http.Headers;
using System.Text.Json;
using Heracles.Api.IntegrationTests.Helpers;
using Heracles.Persistence;
using Heracles.Persistence.SeedData;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Api.IntegrationTests;

/// <summary>
///     Base class for integration tests.
/// </summary>
public abstract class BaseIntegrationTest
{
	protected readonly string NonAdminUserId = UsersDataLoader.Users().Last().UserId;
	protected readonly string AdminUserId = UsersDataLoader.Users().First().UserId;
	protected HttpClient Client;
	private HeraclesWebApplicationFactory _factory;

	[OneTimeSetUp]
	public async Task OneTimeSetUp()
	{
		_factory = new HeraclesWebApplicationFactory();
		await _factory.InitializeAsync();
		Client = _factory.CreateClient();

		// Create a fake JWT token
		var token = JwtTokenHelper.CreateFakeJwtToken();

		// Add the token to the Authorization header
		Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}

	[OneTimeTearDown]
	public async Task OneTimeTearDown()
	{
		Client.Dispose();
		await _factory.DisposeAsync();
	}


	protected AppDbContext GetDbContext()
	{
		var scope = _factory.Services.CreateScope();
		return scope.ServiceProvider.GetRequiredService<AppDbContext>();
	}

	/// <summary>
	///     Handles the failure response of an HTTP groupRequest.
	///     It will show the response content if the response is not successful.
	/// </summary>
	/// <param name="response">The HTTP response message.</param>
	protected void HandleResponseFailure(HttpResponseMessage response)
	{
		if (!response.IsSuccessStatusCode)
		{
			var content = response.Content
				.ReadAsStringAsync()
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();

			Console.WriteLine("=== RESPONSE FAILURE ===");
			Console.WriteLine(response);
			Console.WriteLine("=== CONTENT ===");
			Console.WriteLine(content);

			Assert.Fail("Request failed. See console output for details.");
		}
	}


	public static void DumpApiResponse( HttpResponseMessage response)
	{
		Console.WriteLine("=== API RESPONSE DEBUG ===");
		Console.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");

		var raw = response.Content
			.ReadAsStringAsync()
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		if (string.IsNullOrWhiteSpace(raw))
		{
			Console.WriteLine("<empty body>");
			return;
		}

		try
		{
			var json = JsonSerializer.Deserialize<JsonElement>(raw);
			var pretty = JsonSerializer.Serialize(json, new JsonSerializerOptions
			{
				WriteIndented = true
			});

			Console.WriteLine(pretty);
		}
		catch
		{
			Console.WriteLine(raw);
		}

		Console.WriteLine("==========================");
	}
}
