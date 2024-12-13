using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class CreateWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		DbContext.SaveChanges();

		_handler = new CreateWorkoutSessionCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions().Take(4);

	private CreateWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task CreateWorkoutSessionCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateWorkoutSessionRequest("UniqueName", "Monday", 1, _workoutSessions.First().UserId);
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var workoutSession = await DbContext.WorkoutSessions.FindAsync(result.Value);

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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.DuplicateEntryWithEntityNames(nameof(WorkoutSession))));
	}

	[Test]
	public async Task
		CreateWorkoutSessionCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		var request = new CreateWorkoutSessionRequest("UniqueName", "Monday", 1, _workoutSessions.First().UserId);
		var command = new CreateWorkoutSessionCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(User))));
	}
}
