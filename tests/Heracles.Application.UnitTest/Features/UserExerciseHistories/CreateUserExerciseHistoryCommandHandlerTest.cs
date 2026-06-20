using FluentResults;
using Heracles.Application.Features.UserExerciseHistories.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.UserExerciseHistories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistory")]
public class CreateUserExerciseHistoryCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.Query().ToList();
		_userExercises = UserExercisesRepository.Query().ToList();

		_handler = new CreateUserExerciseHistoryCommandHandler(UserExerciseHistoriesRepository, UsersRepository,
			UserExercisesRepository, CurrentUserServiceMock);
	}

	private List<User> _users;
	private List<UserExercise> _userExercises;
	private CreateUserExerciseHistoryCommandHandler _handler;

	[Test]
	public async Task CreateUserExerciseHistoryCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateUserExerciseHistoryRequest(_userExercises.First().Id, 10, 10, _users.First().UserId);
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseHistory =
			await UserExerciseHistoriesRepository.Query().FirstOrDefaultAsync(x => x.Id == result.Value);

		// Assert

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<int>>());
			Assert.That(result.Value, Is.GreaterThan(0));
			Assert.That(userExerciseHistory, Is.Not.Null);
			Assert.That(userExerciseHistory!.Id, Is.EqualTo(result.Value));
			Assert.That(userExerciseHistory.UserExerciseId, Is.EqualTo(request.UserExerciseId));
			Assert.That(userExerciseHistory.Weight, Is.EqualTo(request.Weight));
			Assert.That(userExerciseHistory.Repetition, Is.EqualTo(request.Repetition));
		});
	}

	[Test]
	public async Task CreateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserIdIsInvalid()
	{
		// Arrange
		var request = new CreateUserExerciseHistoryRequest(_userExercises.First().Id, 10, 10,
			"12345678-1234-1234-1234-123456789012");
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<int>>());
			Assert.That(result.IsFailed, Is.True);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
		});
	}

	[Test]
	public async Task CreateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserExerciseIdIsInvalid()
	{
		// Arrange
		var request = new CreateUserExerciseHistoryRequest(0, 10, 10, _users.First().UserId);
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<int>>());
			Assert.That(result.IsFailed, Is.True);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
		});
	}

	[Test]
	public async Task
		CreateUserExerciseHistoryCommandHandler_ShouldReturnError__WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		var request = new CreateUserExerciseHistoryRequest(_userExercises.First().Id, 10, 10, _users[1].UserId);
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<int>>());
			Assert.That(result.IsFailed, Is.True);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
		});
	}
}
