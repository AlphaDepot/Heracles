using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.WorkoutSessions;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class CreateWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_handler = new CreateWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UsersRepository,
			CurrentUserServiceMock);
	}


	private IEnumerable<WorkoutSession> _workoutSessions;

	private CreateWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task CreateWorkoutSessionCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateWorkoutSessionRequest("UniqueName", "Monday", 1, _workoutSessions.First().UserId);
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var workoutSession = await WorkoutSessionRepository.GetByIdAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(workoutSession, Is.Not.Null);
		Assert.That(workoutSession.Id, Is.EqualTo(result.Value));
	}

	[Test]
	public async Task CreateWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenWorkoutSessionAlreadyExist()
	{
		// Arrange
		var request = new CreateWorkoutSessionRequest(_workoutSessions.First().Name, "Monday", 1,
			_workoutSessions.First().UserId);
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.DuplicateEntry));
	}

	[Test]
	public async Task
		CreateWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		SetAnonymousUser();

		var request = new CreateWorkoutSessionRequest("UniqueName", "Monday", 1, _workoutSessions.First().UserId);
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var handler = new CreateWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UsersRepository,
			CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task CreateWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsInvalid()
	{
		// Arrange
		var request = new CreateWorkoutSessionRequest("UniqueName", "Monday", 1, "Invalid User Id");
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
