using Application.Common.Responses;
using Application.Features.WorkoutSessions;
using Application.Features.WorkoutSessions.Queries;
using Application.UnitTest.TestData;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class WorkoutSessionsByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.WorkoutSessions.AddRange(_workoutSessions);
		DbContext.SaveChanges();

		_handler = new WorkoutSessionsByUserIdQueryHandler(DbContext, HttpContextAccessor);
	}

	//private readonly List<User> _users = UserData.Users();
	private readonly IEnumerable<WorkoutSession> _workoutSessions = UserExerciseData.WorkoutSessions().Take(4);

	private WorkoutSessionsByUserIdQueryHandler _handler;


	[Test]
	public async Task WorkoutSessionByIdQueryHandler_ShouldReturnUserWorkouts()
	{
		// Arrange
		var query = new WorkoutSessionsByUserIdQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<WorkoutSession>>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value.Count, Is.EqualTo(3));
		Assert.That(result.Value.First().UserExercises!.Count(), Is.GreaterThan(0));
	}

	[Test]
	public async Task WorkoutSessionByIdQueryHandler_ShouldReturnEmptyWorkouts_WhenUserHasNone()
	{
		// Arrange
		// using empty user instead of fake user
		HttpContextAccessor.HttpContext = new DefaultHttpContext();
		var query = new WorkoutSessionsByUserIdQuery();

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<WorkoutSession>>>());
		Assert.That(result.Value.Count, Is.EqualTo(0));
	}
}
