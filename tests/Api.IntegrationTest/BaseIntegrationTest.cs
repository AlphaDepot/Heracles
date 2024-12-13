using System.Net.Http.Headers;
using Api.IntegrationTest.Helpers;
using Application.Infrastructure.Data.SeedData;

namespace Api.IntegrationTest;

/// <summary>
/// Base class for integration tests.
/// </summary>
public abstract class BaseIntegrationTest
{
    protected HeraclesWebApplicationFactory Factory;
    protected HttpClient Client;

    //protected const string AdminUserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f22";
    protected readonly string AdminUserId = UsersDataLoader.Users().First().UserId;
    protected const string NonAdminUserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25";

    [OneTimeSetUp]
    public  async Task OneTimeSetUp()
    {
        Factory = new HeraclesWebApplicationFactory();
        await Factory.InitializeAsync();
        Client = Factory.CreateClient();

        // Create a fake JWT token
        var token = JwtTokenHelper.CreateFakeJwtToken();

        // Add the token to the Authorization header
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
	    Client.Dispose();
	    await Factory.DisposeAsync();
    }

    /// <summary>
    /// Handles the failure response of an HTTP request.
    /// It will show the response content if the response is not successful.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    protected void HandleResponseFailure(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            Assert.Fail($"The response is  : {response}, \n The content is: {response.Content.ReadAsStringAsync().Result}");
    }
}
