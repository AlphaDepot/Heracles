using System.Security.Claims;
using System.Text.Json;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;
using Application.UnitTest.TestData;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class DetachUserExerciseToWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		//DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new DetachUserExerciseToWorkoutSessionCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly List<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();

	private DetachUserExerciseToWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var request =
			new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		Console.WriteLine(JsonSerializer.Serialize(result.Error));
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnNotFoundErrorResult_WhenWorkoutSessionNotFound()
	{
		// Arrange
		var request = new DetachUserExerciseToWorkoutSessionRequest(1000, _userExercises.First().Id);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnNotFoundErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var request = new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, 1000);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);
		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserExerciseAlreadyDetached()
	{
		// Arrange
		var newUserExercise = new UserExercise { Id = 40, UserId = _users.First().UserId, ExerciseTypeId = 1 };
		DbContext.UserExercises.Add(newUserExercise);
		await DbContext.SaveChangesAsync();

		var request = new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, newUserExercise.Id);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.BadRequest));
		Assert.That(result.Error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromContextUser()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		var request =
			new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new DetachUserExerciseToWorkoutSessionCommand(request);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserDoesNotOwnWorkoutSession()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = _users.Last().ToClaimsPrincipal();
		}

		var request =
			new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
