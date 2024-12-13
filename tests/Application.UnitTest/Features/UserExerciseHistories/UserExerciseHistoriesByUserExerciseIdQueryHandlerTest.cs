using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExerciseHistories;
using Application.Features.UserExerciseHistories.Queries;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistories")]
public class UserExerciseHistoriesByUserExerciseIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.UserExerciseHistories.AddRange(_userExerciseHistories);
		DbContext.SaveChanges();

		_handler = new UserExerciseHistoriesByUserExerciseIdQueryHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private readonly IEnumerable<UserExerciseHistory> _userExerciseHistories = UserExerciseData.UserExerciseHistories();

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
		//Note: this count is dependent on the seed data in UserExerciseData.UserExerciseHistories
		Assert.That(result.Value.Count, Is.EqualTo(2));
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
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		var userExercise = _userExercises.First();
		var query = new UserExerciseHistoriesByUserExerciseIdQuery(userExercise.Id);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<List<UserExerciseHistory>>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
