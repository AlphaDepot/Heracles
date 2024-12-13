using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExerciseHistories;
using Application.Features.UserExerciseHistories.Commands;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistories")]
public class RemoveUserExerciseHistoryCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.UserExerciseHistories.AddRange(_userExerciseHistories);
		DbContext.SaveChanges();
		_handler = new RemoveUserExerciseHistoryCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private readonly List<UserExerciseHistory> _userExerciseHistories = UserExerciseData.UserExerciseHistories();
	private RemoveUserExerciseHistoryCommandHandler _handler;

	[Test]
	public async Task RemoveUserExerciseHistoryCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveUserExerciseHistoryCommand(_userExerciseHistories.First().Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseHistoryRemoved =
			await DbContext.UserExerciseHistories.FindAsync(_userExerciseHistories.First().Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(userExerciseHistoryRemoved, Is.Null);
	}

	[Test]
	public async Task RemoveUserExerciseHistoryCommandHandler_ShouldReturnErrorResult_WhenUserExerciseHistoryNotFound()
	{
		// Arrange
		var command = new RemoveUserExerciseHistoryCommand(10000);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveUserExerciseHistoryCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		var userExerciseHistory = _userExerciseHistories.Last();
		var command = new RemoveUserExerciseHistoryCommand(userExerciseHistory.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
