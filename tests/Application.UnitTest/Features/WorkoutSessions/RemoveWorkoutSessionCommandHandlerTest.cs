using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class RemoveWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		DbContext.SaveChanges();

		_handler = new RemoveWorkoutSessionCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions().Take(4);

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
		var workoutSessionRemoved = await DbContext.WorkoutSessions.FindAsync(workoutSession.Id);

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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		var workoutSession = _workoutSessions.Last();
		// - Create command
		var command = new RemoveWorkoutSessionCommand(workoutSession.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
