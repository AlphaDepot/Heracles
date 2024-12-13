using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class WorkoutSessionByIdAndUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		DbContext.SaveChanges();

		_handler = new WorkoutSessionByIdAndUserIdQueryHandler(DbContext, HttpContextAccessor);
	}

	//private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions().Take(4);

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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task WorkoutSessionByIdAndUserIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		var workoutSession = _workoutSessions.First();
		var query = new WorkoutSessionByIdAndUserIdQuery(workoutSession.Id);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<WorkoutSession>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
