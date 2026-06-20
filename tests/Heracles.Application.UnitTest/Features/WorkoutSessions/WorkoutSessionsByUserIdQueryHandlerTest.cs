using FluentResults;
using Heracles.Application.Features.WorkoutSessions.Queries;
using Heracles.Domain.Entities;

namespace Heracles.Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "WorkoutSessions")]
public class WorkoutSessionsByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[Test]
	public async Task WorkoutSessionByIdQueryHandler_ShouldReturnUserWorkouts()
	{
		// Arrange
		var query = new WorkoutSessionsByUserIdQuery();

		// Act
		var handler = new WorkoutSessionsByUserIdQueryHandler(WorkoutSessionRepository,
			CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<WorkoutSession>>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value.Count, Is.EqualTo(WorkoutSessionRepository.Query().Count()));
		Assert.That(result.Value.First().UserExercises!.Count(), Is.GreaterThan(0));
	}

	[Test]
	public async Task WorkoutSessionByIdQueryHandler_ShouldReturnEmptyWorkouts_WhenUserHasNone()
	{
		// Arrange
		// Add A new user that guarantees no workouts
		var user = new User
		{
			Email = "null@null.local",
			UserId = "null"
		};

		await UsersRepository.AddAsync(user);
		await UsersRepository.SaveChangesAsync();

		SetCurrentUser(user);
		var query = new WorkoutSessionsByUserIdQuery();

		// Act
		var handler = new WorkoutSessionsByUserIdQueryHandler(WorkoutSessionRepository,
			CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		result.LogFailedResult();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<WorkoutSession>>>());
		Assert.That(result.Value.Count, Is.EqualTo(0));
	}
}
