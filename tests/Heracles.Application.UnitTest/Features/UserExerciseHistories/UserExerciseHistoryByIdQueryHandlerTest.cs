using FluentResults;
using Heracles.Application.Features.UserExerciseHistories.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistories")]
public class UserExerciseHistoryByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExerciseHistories = UserExerciseHistoriesRepository.Query().ToList();
		_handler = new UserExerciseHistoryByIdQueryHandler(UserExerciseHistoriesRepository, CurrentUserServiceMock);
	}

	private List<UserExerciseHistory> _userExerciseHistories;

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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		SetAnonymousUser();

		var query = new UserExerciseHistoryByIdQuery(1);


		// Act
		var handler = new UserExerciseHistoryByIdQueryHandler(UserExerciseHistoriesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}


	[Test]
	public async Task UserExerciseHistoryByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthorized_AndNotAdmin()
	{
		// Arrange
		SetCurrentUser(UsersRepository.Query().ToList().Last(), false);
		var query = new UserExerciseHistoryByIdQuery(1);

		// Act
		var handler = new UserExerciseHistoryByIdQueryHandler(UserExerciseHistoriesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExerciseHistory>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
