using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.WorkoutSessions;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class AttachUserExerciseToWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_userExercises = UserExercisesRepository.Query().ToList();
		_handler = new AttachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UserExercisesRepository, CurrentUserServiceMock);
	}


	private IEnumerable<WorkoutSession> _workoutSessions;
	private List<UserExercise> _userExercises;

	private AttachUserExerciseToWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// Remove the user exercise from the workout session
		var session = new WorkoutSession
		{
			Name = "Test Workout Session",
			UserId = CurrentUserServiceMock.UserId
		};
		await WorkoutSessionRepository.AddAsync(session);
		await WorkoutSessionRepository.SaveChangesAsync();


		var request = new AttachUserExerciseToWorkoutSessionRequest(_userExercises[2].Id, session.Id);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		result.LogFailedResult();

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
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnNotFoundErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var request = new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, 1000);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserExerciseAlreadyAttached()
	{
		// Arrange
		var request =
			new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.DuplicateEntry));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromContextUser()
	{
		// Arrange
		SetAnonymousUser();
		var request =
			new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var handler = new AttachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UserExercisesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task
		AttacheUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserDoesNotOwnWorkoutSession()
	{
		// Arrange
		SetAnonymousUser();

		var request =
			new AttachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var handler = new AttachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UserExercisesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
