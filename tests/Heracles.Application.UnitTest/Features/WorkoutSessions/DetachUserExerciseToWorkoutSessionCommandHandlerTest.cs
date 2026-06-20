using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.WorkoutSessions;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class DetachUserExerciseToWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.Query().ToList();
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_userExercises = UserExercisesRepository.Query().ToList();
		_handler = new DetachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UserExercisesRepository, CurrentUserServiceMock);
	}

	private List<User> _users;
	private List<WorkoutSession> _workoutSessions;
	private List<UserExercise> _userExercises;

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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserExerciseAlreadyDetached()
	{
		// Arrange
		var newUserExercise = new UserExercise { Id = 40, UserId = _users.First().UserId, ExerciseTypeId = 1 };

		await UserExercisesRepository.AddAsync(newUserExercise);
		await UserExercisesRepository.SaveChangesAsync();

		var request = new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, newUserExercise.Id);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.BadRequest));
		Assert.That(result.Errors.First().Metadata["StatusCode"], Is.EqualTo(400));
	}

	[Test]
	public async Task
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromContextUser()
	{
		SetAnonymousUser();

		var request =
			new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new DetachUserExerciseToWorkoutSessionCommand(request);


		// Act
		var handler = new DetachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
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
		DetachUserExerciseToWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserDoesNotOwnWorkoutSession()
	{
		SetAnonymousUser();

		var request =
			new DetachUserExerciseToWorkoutSessionRequest(_workoutSessions.First().Id, _userExercises.First().Id);
		var command =
			new DetachUserExerciseToWorkoutSessionCommand(request);

		// Act
		var handler = new DetachUserExerciseToWorkoutSessionCommandHandler(WorkoutSessionRepository,
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
