using System.Net.Http.Headers;
using Heracles.API.IntegrationTest.Helpers;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest;

/// <summary>
/// Base class for integration tests.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<HeraclesWebApplicationFactory>
{
    protected readonly HeraclesWebApplicationFactory _factory;
    
    protected readonly ITestOutputHelper _console;
    protected readonly HttpClient _client;
    
    protected const string AdminUserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f21";
    protected const string NonAdminUserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25";

    protected BaseIntegrationTest(HeraclesWebApplicationFactory factory, ITestOutputHelper console)
    {
        _factory = factory;
        _console = console;
        _client = _factory.CreateClient();
        
        // Create a fake JWT token
        var token = JwtTokenHelper.CreateFakeJwtToken();

        // Add the token to the Authorization header
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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