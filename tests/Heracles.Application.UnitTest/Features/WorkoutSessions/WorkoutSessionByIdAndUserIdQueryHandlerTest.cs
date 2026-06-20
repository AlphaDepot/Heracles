using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class WorkoutSessionByIdAndUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_workoutSessions = WorkoutSessionRepository.Query().ToList();
		_handler = new WorkoutSessionByIdAndUserIdQueryHandler(WorkoutSessionRepository, CurrentUserServiceMock);
	}

	private IEnumerable<WorkoutSession> _workoutSessions;

	private WorkoutSessionByIdAndUserIdQueryHandler _handler;

	[Test]
	public async Task WorkoutSessionByIdAndUserIdQueryHandler_ShouldReturnWorkoutSession()
	{
		// Arrange
		var workoutSession = _workoutSessions.First();
		var query = new WorkoutSessionByIdAndUserIdQuery(workoutSession.Id);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<WorkoutSession>>());
		Assert.That(result.Value.Id, Is.EqualTo(workoutSession.Id));
		Assert.That(result.Value.UserId, Is.EqualTo(workoutSession.UserId));
	}

	[Test]
	public async Task WorkoutSessionByIdAndUserIdQueryHandler_ShouldReturnErrorResult_WhenWorkoutSessionNotFound()
	{
		// Arrange
		var query = new WorkoutSessionByIdAndUserIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<WorkoutSession>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task WorkoutSessionByIdAndUserIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		SetAnonymousUser();

		var workoutSession = _workoutSessions.First();
		var query = new WorkoutSessionByIdAndUserIdQuery(workoutSession.Id);

		// Act
		var handler = new WorkoutSessionByIdAndUserIdQueryHandler(WorkoutSessionRepository,
			CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<WorkoutSession>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
