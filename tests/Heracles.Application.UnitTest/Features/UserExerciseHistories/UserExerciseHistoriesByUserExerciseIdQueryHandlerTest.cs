using FluentResults;
using Heracles.Application.Features.UserExerciseHistories.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistories")]
public class UserExerciseHistoriesByUserExerciseIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExercises = UserExercisesRepository.Query().ToList();
		_handler = new UserExerciseHistoriesByUserExerciseIdQueryHandler(UserExerciseHistoriesRepository,
			CurrentUserServiceMock);
	}


	private List<UserExercise> _userExercises;

	private UserExerciseHistoriesByUserExerciseIdQueryHandler _handler;

	[Test]
	public async Task UserExerciseHistoriesByUserExerciseIdQueryHandler_ShouldReturnUserExerciseHistories()
	{
		// Arrange
		var userExercise = _userExercises.First();
		var query = new UserExerciseHistoriesByUserExerciseIdQuery(userExercise.Id);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<UserExerciseHistory>>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value, Is.InstanceOf<List<UserExerciseHistory>>());
		Console.WriteLine(result.Value.Count);
		Assert.That(result.Value.First().UserExerciseId, Is.EqualTo(userExercise.Id));
	}

	[Test]
	public async Task
		UserExerciseHistoriesByUserExerciseIdQueryHandler_ShouldReturnEmptyList_WhenUserExerciseHistoriesNotFound()
	{
		// Arrange
		var query = new UserExerciseHistoriesByUserExerciseIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<UserExerciseHistory>>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value, Is.InstanceOf<List<UserExerciseHistory>>());
		Assert.That(result.Value.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task UserExerciseHistoriesByUserExerciseIdQueryHandler_ShouldReturnEmptyList_WhenUserNotAuthenticated()
	{
		// Arrange
		SetAnonymousUser();

		var userExercise = _userExercises.First();
		var query = new UserExerciseHistoriesByUserExerciseIdQuery(userExercise.Id);

		// Act
		var handler =
			new UserExerciseHistoriesByUserExerciseIdQueryHandler(UserExerciseHistoriesRepository,
				CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<UserExerciseHistory>>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
