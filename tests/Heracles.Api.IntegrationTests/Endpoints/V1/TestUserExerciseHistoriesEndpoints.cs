using System.Net.Http.Json;
using Heracles.Application.Features.UserExerciseHistories.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.UserExerciseHistories;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the UserExerciseHistoriesController.
/// </summary>
public class TestUserExerciseHistoriesEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.UserExerciseHistories;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededUserExerciseHistories()
	{
		using var context = GetDbContext();

		var count = context.UserExerciseHistories.Count();

		Console.WriteLine("User Exercise Histories count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetUserExerciseHistories endpoint returns a list of UserExerciseHistories.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task GetUserExerciseHistories_ByUserExerciseId_ReturnsUserExerciseHistories()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/by-user-exercise/1");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserExerciseHistory>>>();
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetUserExerciseHistoryById endpoint returns the correct UserExerciseHistory.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task GetUserExerciseHistoryById_ReturnsUserExerciseHistory()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserExerciseHistory>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(1));
	}

	/// <summary>
	///     Test to ensure that the CreateUserExerciseHistory endpoint creates a new UserExerciseHistory.
	/// </summary>
	[Test]
	[Order(1)]
	public async Task CreateUserExerciseHistory_ReturnsCreatedUserExerciseHistory()
	{
		// Arrange
		var userExerciseHistory = new CreateUserExerciseHistoryRequest(2, 50, 10, AdminUserId);

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, userExerciseHistory);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateUserExerciseHistory endpoint updates an existing UserExerciseHistory.
	/// </summary>
	[Test]
	[Order(2)]
	public async Task UpdateUserExerciseHistory_ReturnsUpdatedUserExerciseHistory()
	{
		// Arrange
		var existingUserExerciseHistory = await Client.GetFromJsonAsync<ApiResponse<UserExerciseHistory>>($"{BaseUrl}/1");
		var userExerciseHistory = new UpdateUserExerciseHistoryRequest
		{
			Id = 1,
			UserId = AdminUserId,
			UserExerciseId = 2,
			Concurrency = existingUserExerciseHistory?.Data?.Concurrency!,
			Weight = 70,
			Repetition = 10
		};

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/1", userExerciseHistory);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the DeleteUserExerciseHistory endpoint deletes a UserExerciseHistory correctly.
	/// </summary>
	[Test]
	[Order(4)]
	public async Task DeleteUserExerciseHistory_ReturnsDeletedUserExerciseHistory()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
