using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.WorkoutSessions;
using Heracles.Shared.Utilities;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class UpdateWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_handler = new UpdateWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UsersRepository, CurrentUserServiceMock);
	}


	private IEnumerable<WorkoutSession> _workoutSessions;

	private UpdateWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnTrue_WhenUpdateIsSuccessful()
	{
		// Arrange
		var existingWorkoutSession = await WorkoutSessionRepository.GetByIdAsync(_workoutSessions.First().Id);
		var request = new UpdateWorkoutSessionRequest
		{
			Id = existingWorkoutSession!.Id,
			UserId = existingWorkoutSession.UserId,
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = existingWorkoutSession.Concurrency
		};

		var command = new UpdateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var workoutSession = await WorkoutSessionRepository.GetByIdAsync(existingWorkoutSession.Id);
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(workoutSession, Is.Not.Null);
		Assert.That(workoutSession.Name, Is.EqualTo(request.Name));
		Assert.That(workoutSession.DayOfWeek,
			Is.EqualTo(DayOfWeekBuilder.GetDayOfWeek(request.DayOfWeek) ?? DayOfWeek.Sunday));
		Assert.That(workoutSession.SortOrder, Is.EqualTo(request.SortOrder));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenWorkoutSessionDoesNotExist()
	{
		// Arrange
		var request = new UpdateWorkoutSessionRequest
		{
			Id = 1000,
			UserId = _workoutSessions.First().UserId,
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = Guid.NewGuid().ToString()
		};

		var command = new UpdateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenConcurrencyError()
	{
		// Arrange
		var existingWorkoutSession = await WorkoutSessionRepository.GetByIdAsync(_workoutSessions.First().Id);
		var request = new UpdateWorkoutSessionRequest
		{
			Id = existingWorkoutSession!.Id,
			UserId = existingWorkoutSession.UserId,
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = Guid.NewGuid().ToString()
		};

		var command = new UpdateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.ConcurrencyError));
	}

	[Test]
	public async Task
		UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		var existingWorkoutSession = await WorkoutSessionRepository.GetByIdAsync(_workoutSessions.First().Id);
		var request = new UpdateWorkoutSessionRequest
		{
			Id = existingWorkoutSession!.Id,
			UserId = existingWorkoutSession.UserId,
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = existingWorkoutSession.Concurrency
		};

		var command = new UpdateWorkoutSessionCommand(request);
		SetAnonymousUser();
		// Act
		var handler = new UpdateWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UsersRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserIsNotAuthorized()
	{
		// Arrange
		var existingWorkoutSession = await WorkoutSessionRepository.GetByIdAsync(_workoutSessions.First().Id);
		var request = new UpdateWorkoutSessionRequest
		{
			Id = existingWorkoutSession!.Id,
			UserId = existingWorkoutSession.UserId,
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = existingWorkoutSession.Concurrency
		};

		var command = new UpdateWorkoutSessionCommand(request);
		SetAnonymousUser();
		// Act
		var handler = new UpdateWorkoutSessionCommandHandler(WorkoutSessionRepository,
			UsersRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserDoesNotExist()
	{
		// Arrange
		var existingWorkoutSession = await WorkoutSessionRepository.GetByIdAsync(_workoutSessions.First().Id);
		var request = new UpdateWorkoutSessionRequest
		{
			Id = existingWorkoutSession!.Id,
			UserId = "InvalidUserId",
			Name = "ChangedName",
			DayOfWeek = "Monday",
			SortOrder = 1,
			Concurrency = existingWorkoutSession.Concurrency
		};

		var command = new UpdateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
