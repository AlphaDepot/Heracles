using System.Net.Http.Headers;
using Api.IntegrationTest.Helpers;
using Application.Infrastructure.Data.SeedData;

namespace Api.IntegrationTest;

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

	/// <summary>
	///     Handles the failure response of an HTTP request.
	///     It will show the response content if the response is not successful.
	/// </summary>
	/// <param name="response">The HTTP response message.</param>
	protected void HandleResponseFailure(HttpResponseMessage response)
	{
		if (!response.IsSuccessStatusCode)
		{
			Assert.Fail(
				$"The response is  : {response}, \n The content is: {response.Content.ReadAsStringAsync().Result}");
		}
	}
}
