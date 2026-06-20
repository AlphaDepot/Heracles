using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class RemoveWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_handler = new RemoveWorkoutSessionCommandHandler(WorkoutSessionRepository, CurrentUserServiceMock);
	}


	private IEnumerable<WorkoutSession> _workoutSessions;

	private RemoveWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task RemoveWorkoutSessionCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var workoutSession = _workoutSessions.First();
		// - Create command
		var command = new RemoveWorkoutSessionCommand(workoutSession.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var workoutSessionRemoved = await WorkoutSessionRepository.GetByIdAsync(workoutSession.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(workoutSessionRemoved, Is.Null);
	}

	[Test]
	public async Task RemoveWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenWorkoutSessionNotFound()
	{
		// Arrange
		var command = new RemoveWorkoutSessionCommand(10000);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		SetAnonymousUser();
		var workoutSession = _workoutSessions.Last();
		// - Create command
		var command = new RemoveWorkoutSessionCommand(workoutSession.Id);

		// Act
		var handler = new RemoveWorkoutSessionCommandHandler(WorkoutSessionRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
