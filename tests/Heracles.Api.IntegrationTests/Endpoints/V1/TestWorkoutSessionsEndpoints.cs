using System.Net.Http.Json;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.WorkoutSessions;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     Represents a controller for testing workout sessions.
/// </summary>
public class TestWorkoutSessionsEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.WorkoutSessions;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededWorkoutSessions()
	{
		using var context = GetDbContext();

		var count = context.WorkoutSessions.Count();

		Console.WriteLine("Workout session count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
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

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<WorkoutSession>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data, Is.Not.Empty);
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

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<WorkoutSession>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(1));
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

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateWorkoutSession endpoint updates a workout session correctly.
	/// </summary>
	[Test]
	[Order(2)]
	public async Task UpdateWorkoutSession_ReturnsUpdatedWorkoutSession()
	{
		// Arrange
		var newName = "Test Workout Session Updated";
		var existingWorkoutSession = await Client.GetFromJsonAsync<ApiResponse<WorkoutSession>>($"{BaseUrl}/1");
		var workoutSession = new UpdateWorkoutSessionRequest
		{
			Id = 1,
			UserId = AdminUserId,
			Name = newName,
			Concurrency = existingWorkoutSession!.Data!.Concurrency,
			DayOfWeek = DayOfWeek.Friday.ToString()
		};

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/{workoutSession.Id}", workoutSession);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
		var result = await Client.GetFromJsonAsync<ApiResponse<WorkoutSession>>($"{BaseUrl}/1");
		Assert.That(result.Data.Name, Is.EqualTo(newName));
	}

	/// <summary>
	///     Test to ensure that the AddExerciseToWorkoutSession endpoint adds an exercise to a workout session correctly.
	/// </summary>
	[Test]
	[Order(3)]
	public async Task AddExerciseToWorkoutSession_ReturnsUpdatedWorkoutSession()
	{
		// Arrange
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
		await using var context = GetDbContext();

		// Load the workout session with its exercises
		var session = context.WorkoutSessions
			.Include(x => x.UserExercises)
			.First(x => x.Id == 1);

		// Pick the first exercise actually attached to this session
		var exercise = session.UserExercises.FirstOrDefault();
		Assert.That(exercise, Is.Not.Null, "Workout session has no exercises to detach.");

		var request = new DetachUserExerciseToWorkoutSessionRequest(
			session.Id,
			exercise.Id
		);

		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/{session.Id}/remove",
			request);
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
