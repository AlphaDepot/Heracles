using System.Net.Http.Json;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
///     Represents a controller for testing workout sessions.
/// </summary>
public class TestWorkoutSessionsController : BaseIntegrationTest
{
	private const string BaseUrl = "/api/workoutsessions";

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	/// <summary>
	///     Test to ensure that the GetWorkoutSessions endpoint returns workout sessions.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task GetWorkoutSessions_ReturnsWorkoutSessions()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<List<WorkoutSession>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetWorkoutSessionById endpoint returns the correct workout session.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task GetWorkoutSessionById_ReturnsWorkoutSession()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<WorkoutSession>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(1));
	}

	/// <summary>
	///     Test to ensure that the CreateWorkoutSession endpoint creates a new workout session.
	/// </summary>
	[Test]
	[Order(1)]
	public async Task CreateWorkoutSession_ReturnsCreatedWorkoutSession()
	{
		// Arrange
		var workoutSession = new CreateWorkoutSessionRequest("Test Workout Session", "Friday", 1, AdminUserId);
		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, workoutSession);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<int>();

		// Assert
		Assert.That(result, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateWorkoutSession endpoint updates a workout session correctly.
	/// </summary>
	[Test]
	[Order(2)]
	public async Task UpdateWorkoutSession_ReturnsUpdatedWorkoutSession()
	{
		// Arrange
		var existingWorkoutSession = await Client.GetFromJsonAsync<WorkoutSession>($"{BaseUrl}/1");
		var workoutSession = new UpdateWorkoutSessionRequest
		{
			Id = 1,
			UserId = AdminUserId,
			Name = "Test Workout Session Updated",
			Concurrency = existingWorkoutSession!.Concurrency,
			DayOfWeek = DayOfWeek.Friday.ToString()
		};

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/{workoutSession.Id}", workoutSession);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the AddExerciseToWorkoutSession endpoint adds an exercise to a workout session correctly.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task AddExerciseToWorkoutSession_ReturnsUpdatedWorkoutSession()
	{
		// Arrange
		//public record AttachUserExerciseToWorkoutSessionRequest(int UserExerciseId, int WorkoutSessionId);

		var workoutSessionExercise = new AttachUserExerciseToWorkoutSessionRequest(2, 2);
		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/{workoutSessionExercise.WorkoutSessionId}/add",
			workoutSessionExercise);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the RemoveExerciseFromWorkoutSession endpoint removes an exercise from a workout session
	///     correctly.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task RemoveExerciseFromWorkoutSession_ReturnsUpdatedWorkoutSession()
	{
		// Arrange
		var workoutSessionExercise = new DetachUserExerciseToWorkoutSessionRequest(2, 2);
		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/{workoutSessionExercise.WorkoutSessionId}/remove",
			workoutSessionExercise);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the DeleteWorkoutSession endpoint deletes a workout session correctly.
	/// </summary>
	[Test]
	[Order(4)]
	public async Task DeleteWorkoutSession_ReturnsDeletedWorkoutSession()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
