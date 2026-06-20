using System.Net.Http.Json;
using Heracles.Application.Features.Users.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.Users;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class is used to test the functionality of the Users API controller.
/// </summary>
public class TestUsersEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.Users;

	[Test, Order(1)]
	public void Database_ShouldContainSeededUsers()
	{
		using var context = GetDbContext();

		var count = context.Users.Count();

		Console.WriteLine("User count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetUserById endpoint returns the correct user.
	/// </summary>
	[Test]
	[Order(2)]
	public async Task GetUserById_ReturnsUser()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/{AdminUserId}");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<User>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.UserId, Is.EqualTo(AdminUserId));
	}

	/// <summary>
	///     Test to ensure that the CreateUser endpoint correctly creates a user.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task CreateUser_ReturnsCreatedUser()
	{
		// Arrange
		var guid = Guid.NewGuid().ToString();
		var user = new CreateUserRequest(guid, "test@jotest.com", true);

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, user);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data, Is.Not.EqualTo(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateUser endpoint correctly updates a user.
	/// </summary>
	[Test]
	[Order(4)]
	public async Task UpdateUser_ReturnsUpdatedUser()
	{
		// Arrange
		var newEmail = "john@outlook.com";
		var user = new UpdateUserRequest(AdminUserId, newEmail, true);

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/1", user);
		HandleResponseFailure(response);

		// Assert

		var res = await Client.GetAsync($"{BaseUrl}/{AdminUserId}");
		var result = await res.Content.ReadFromJsonAsync<ApiResponse<User>>();
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Email, Is.EqualTo(newEmail));
		Assert.That(result.Data.Id, Is.Not.EqualTo(0));
	}

	/// <summary>
	///     Test to ensure that the DeleteUser endpoint correctly deletes a user.
	/// </summary>
	[Test]
	[Order(5)]
	public async Task DeleteUser_ReturnsDeletedUser()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	[Test]
	[Order(5)]
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

	[Test]
	[Order(3)]
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
