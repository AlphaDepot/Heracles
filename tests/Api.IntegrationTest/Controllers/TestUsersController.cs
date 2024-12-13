using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Microsoft.AspNetCore.Http;


namespace Api.IntegrationTest.Controllers;

/// <summary>
/// This class is used to test the functionality of the Users API controller.
/// </summary>
public class TestUsersController : BaseIntegrationTest
{
    private const string BaseUrl = "/api/users";

    /// <summary>
    /// Test to ensure that the GetUserById endpoint returns the correct user.
    /// </summary>
    [Test, Order(1)]
    public async Task GetUserById_ReturnsUser()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/{AdminUserId}");
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<User>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(AdminUserId));
    }

    /// <summary>
    /// Test to ensure that the CreateUser endpoint correctly creates a user.
    /// </summary>
    [Test, Order(2)]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
       var user = new CreateUserRequest(guid, "test@jotest.com", true);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, user);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the UpdateUser endpoint correctly updates a user.
    /// </summary>
    [Test, Order(2)]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange
        var user = new UpdateUserRequest(AdminUserId, "john@outlook.com", true);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/1", user);
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteUser endpoint correctly deletes a user.
    /// </summary>
    [Test, Order(3)]
    public async Task DeleteUser_ReturnsDeletedUser()
    {
        // Arrange
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/1");
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Test, Order(4)]
    public async Task CreateOrUpdate_ReturnsCreatedUser()
    {
	    // Arrange
		// Note: DeleteUser_ReturnsDeletedUser must be run before this test
	    var user = new CreateOrUpdateRequest(AdminUserId, "john@outlook.com", true);

	    // Act
	    var response = await Client.PatchAsJsonAsync(BaseUrl, user);
	    HandleResponseFailure(response);
	    // Assert
	    response.EnsureSuccessStatusCode();
    }

    [Test, Order(2)]
    public async Task CreateOrUpdate_ReturnsUpdatedUser()
    {
	    // Arrange
	    var user = new CreateOrUpdateRequest(AdminUserId, "john@outlook.com", true);
	    // Act
	    var response = await Client.PatchAsJsonAsync(BaseUrl, user);
	    HandleResponseFailure(response);
	    // Assert
	    response.EnsureSuccessStatusCode();
    }

}
