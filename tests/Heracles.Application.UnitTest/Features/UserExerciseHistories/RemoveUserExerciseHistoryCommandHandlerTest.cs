using FluentResults;
using Heracles.Application.Features.UserExerciseHistories.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistories")]
public class RemoveUserExerciseHistoryCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExerciseHistories = UserExerciseHistoriesRepository.Query().ToList();
		_handler = new RemoveUserExerciseHistoryCommandHandler(UserExerciseHistoriesRepository, CurrentUserServiceMock);
	}

	private List<UserExerciseHistory> _userExerciseHistories;
	private RemoveUserExerciseHistoryCommandHandler _handler;

	[Test]
	public async Task RemoveUserExerciseHistoryCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveUserExerciseHistoryCommand(_userExerciseHistories.First().Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseHistoryRemoved =
			await UserExerciseHistoriesRepository.Query()
				.FirstOrDefaultAsync(x => x.Id == _userExerciseHistories.First().Id);

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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveUserExerciseHistoryCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		SetAnonymousUser();
		var userExerciseHistory = _userExerciseHistories.Last();
		var command = new RemoveUserExerciseHistoryCommand(userExerciseHistory.Id);

		// Act
		var handler =
			new RemoveUserExerciseHistoryCommandHandler(UserExerciseHistoriesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
