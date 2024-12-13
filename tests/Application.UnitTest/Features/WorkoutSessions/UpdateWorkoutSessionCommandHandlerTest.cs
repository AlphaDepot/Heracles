using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Common.Utilities;
using Application.Features.Users;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class UpdateWorkoutSessionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		DbContext.SaveChanges();

		_handler = new UpdateWorkoutSessionCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions().Take(4);

	private UpdateWorkoutSessionCommandHandler _handler;

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnTrue_WhenUpdateIsSuccessful()
	{
		// Arrange
		var existingWorkoutSession = await DbContext.WorkoutSessions.FindAsync(_workoutSessions.First().Id);
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
		var workoutSession = await DbContext.WorkoutSessions.FindAsync(existingWorkoutSession.Id);

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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(WorkoutSession))));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenConcurrencyError()
	{
		// Arrange
		var existingWorkoutSession = await DbContext.WorkoutSessions.FindAsync(_workoutSessions.First().Id);
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.ConcurrencyError));
	}

	[Test]
	public async Task
		UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		var existingWorkoutSession = await DbContext.WorkoutSessions.FindAsync(_workoutSessions.First().Id);
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
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserIsNotAuthorized()
	{
		// Arrange
		var existingWorkoutSession = await DbContext.WorkoutSessions.FindAsync(_workoutSessions.First().Id);
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
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = null!;
		}

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateWorkoutSessionCommandHandler_ShouldReturnFailureResult_WhenUserDoesNotExist()
	{
		// Arrange
		var existingWorkoutSession = await DbContext.WorkoutSessions.FindAsync(_workoutSessions.First().Id);
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(User))));
	}
}
