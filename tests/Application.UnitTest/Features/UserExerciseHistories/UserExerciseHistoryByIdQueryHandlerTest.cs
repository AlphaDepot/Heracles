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
public class UserExerciseHistoryByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.UserExerciseHistories.AddRange(_userExerciseHistories);
		DbContext.SaveChanges();

		_handler = new UserExerciseHistoryByIdQueryHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private readonly IEnumerable<UserExerciseHistory> _userExerciseHistories = UserExerciseData.UserExerciseHistories();

	private UserExerciseHistoryByIdQueryHandler _handler;

	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnUserExerciseHistory()
	{
		// Arrange
		var userExerciseHistory = _userExerciseHistories.First();
		var query = new UserExerciseHistoryByIdQuery(userExerciseHistory.Id);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.Value.Id, Is.EqualTo(userExerciseHistory.Id));
		Assert.That(result.Value.UserId, Is.EqualTo(userExerciseHistory.UserId));
	}

	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnErrorResult_WhenUserExerciseHistoryNotFound()
	{
		// Arrange
		var query = new UserExerciseHistoryByIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal();
		}

		var query = new UserExerciseHistoryByIdQuery(1);


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}


	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthorized_AndNotAdmin()
	{
		// Arrange
		var query = new UserExerciseHistoryByIdQuery(1);
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, _users.Last().UserId)
			}));
		}

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
