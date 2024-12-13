using System.Net.Http.Json;
using Application.Features.UserExerciseHistories;
using Application.Features.UserExerciseHistories.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
///     This class contains integration tests for the UserExerciseHistoriesController.
/// </summary>
public class TestUserExerciseHistoriesController : BaseIntegrationTest
{
	private const string BaseUrl = "/api/UserExerciseHistories";

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
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

		var result = await response.Content.ReadFromJsonAsync<List<UserExerciseHistory>>();
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.Not.Empty);
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

		var result = await response.Content.ReadFromJsonAsync<UserExerciseHistory>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(1));
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

		var result = await response.Content.ReadFromJsonAsync<int>();

		// Assert
		Assert.That(result, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateUserExerciseHistory endpoint updates an existing UserExerciseHistory.
	/// </summary>
	[Test]
	[Order(2)]
	public async Task UpdateUserExerciseHistory_ReturnsUpdatedUserExerciseHistory()
	{
		// Arrange
		var existingUserExerciseHistory = await Client.GetFromJsonAsync<UserExerciseHistory>($"{BaseUrl}/1");
		var userExerciseHistory = new UpdateUserExerciseHistoryRequest
		{
			Id = 1,
			UserId = AdminUserId,
			UserExerciseId = 2,
			Concurrency = existingUserExerciseHistory?.Concurrency!,
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
