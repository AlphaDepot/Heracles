using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;
using Application.UnitTest.TestData;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class AttachUserExerciseToWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		//DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new AttachUserExerciseToWorkoutSessionCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();

	private AttachUserExerciseToWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// Remove the user exercise from the workout session
		var workoutSession = DbContext.WorkoutSessions.Include(workoutSession => workoutSession.UserExercises).First();
		workoutSession.UserExercises?.Remove(workoutSession.UserExercises.First());
		await DbContext.SaveChangesAsync();
		Task.Delay(1).GetAwaiter().GetResult(); // wait for the user exercise to be removed>
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises[2].Id);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnNotFoundErrorResult_WhenWorkoutSessionNotFound()
	{
		// Arrange
		var request = new AttachUserExerciseToWorkoutSessionRequest(1000, _userExercises.First().Id);
		var command = new AttachUserExerciseToWorkoutSessionCommand( request);

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
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnNotFoundErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, 1000);
		var command = new AttachUserExerciseToWorkoutSessionCommand( request);

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
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserExerciseAlreadyAttached()
	{
		// Arrange
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new AttachUserExerciseToWorkoutSessionCommand( request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.DuplicateEntryWithEntityNames(nameof(UserExercise))));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromContextUser()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new AttachUserExerciseToWorkoutSessionCommand( request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserDoesNotOwnWorkoutSession()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = _users.Last().ToClaimsPrincipal();
		}
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command = new AttachUserExerciseToWorkoutSessionCommand( request);

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
