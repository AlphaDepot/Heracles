using System.Net.Http.Json;
using Heracles.Application.Features.UserExercises.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.UserExercises;
using Heracles.Shared.Responses;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the UserExercisesController.
/// </summary>
public class TestUserExercisesEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.UserExercises;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededUserExercises()
	{
		using var context = GetDbContext();

		var count = context.UserExercises.Count();

		Console.WriteLine("User Exercises count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetUserExercises endpoint returns a list of UserExercises.
	/// </summary>
	[Test]
	public async Task GetUserExercises_ReturnsUserExercises()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<UserExercise>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetUserExerciseById endpoint returns the correct UserExercise.
	/// </summary>
	[Test]
	public async Task GetUserExerciseById_ReturnsUserExercise()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserExercise>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}

	/// <summary>
	///     Test to ensure that the CreateUserExercise endpoint creates a new UserExercise.
	/// </summary>
	[Test]
	public async Task CreateUserExercise_ReturnsCreatedUserExercise()
	{
		// Arrange
		var userExercise = new CreateUserExerciseRequest
		{
			UserId = AdminUserId,
			ExerciseTypeId = 2,
			StaticResistance = 10,
			PercentageResistance = 10,
			CurrentWeight = 10,
			PersonalRecord = 10,
			DurationInSeconds = 10,
			SortOrder = 10,
			Repetitions = 10,
			Sets = 10,
			Timed = true,
			BodyWeight = true
		};

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, userExercise);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that creating a UserExercise with the same exercise id and user id triggers the version increment.
	/// </summary>
	[Test]
	public async Task CreateUserExercise_ReturnsCreatedUserExerciseWithVersionIncrement()
	{
		// Arrange
		var userExercise = new CreateUserExerciseRequest
		{
			UserId = AdminUserId,
			ExerciseTypeId = 2,
			StaticResistance = 10,
			PercentageResistance = 10,
			CurrentWeight = 10,
			PersonalRecord = 10,
			DurationInSeconds = 10,
			SortOrder = 10,
			Repetitions = 10,
			Sets = 10,
			Timed = true,
			BodyWeight = true
		};
		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, userExercise);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateUserExercise endpoint updates a UserExercise correctly.
	/// </summary>
	[Test]
	public async Task UpdateUserExercise_ReturnsUpdatedUserExercise()
	{
		// Arrange
		var existingUserExercise = await Client.GetFromJsonAsync<ApiResponse<UserExercise>>($"{BaseUrl}/2");
		var userExercise = new UpdateUserExerciseRequest
		{
			Id = 2,
			Concurrency = existingUserExercise!.Data!.Concurrency!,
			StaticResistance = 10,
			PercentageResistance = 10,
			CurrentWeight = 10,
			PersonalRecord = 10,
			DurationInSeconds = 10,
			SortOrder = 10,
			Repetitions = 10,
			Sets = 10,
			Timed = true,
			BodyWeight = true
		};
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", userExercise);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the DeleteUserExercise endpoint deletes a UserExercise correctly.
	/// </summary>
	[Test]
	public async Task DeleteUserExercise_ReturnsDeletedUserExercise()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
