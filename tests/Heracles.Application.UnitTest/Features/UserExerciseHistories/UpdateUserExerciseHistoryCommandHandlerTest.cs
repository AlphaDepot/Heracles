using FluentResults;
using Heracles.Application.Features.UserExerciseHistories.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.UserExerciseHistories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistory")]
public class UpdateUserExerciseHistoryCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExerciseHistories = UserExerciseHistoriesRepository.Query().ToList();
		_handler = new UpdateUserExerciseHistoryCommandHandler(UserExerciseHistoriesRepository, UsersRepository,
			UserExercisesRepository, CurrentUserServiceMock);

		_userExerciseHistory =
			UserExerciseHistoriesRepository.Query().FirstOrDefault(x => x.Id == _userExerciseHistories.First().Id);
		if (_userExerciseHistory == null)
		{
			throw new InvalidOperationException("UserExerciseHistory not found in the database");
		}
	}

	private List<UserExerciseHistory> _userExerciseHistories;
	private UpdateUserExerciseHistoryCommandHandler _handler;

	private UserExerciseHistory? _userExerciseHistory;


	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnIntId()
	{
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = _userExerciseHistory!.Id,
			Concurrency = _userExerciseHistory.Concurrency!,
			UserExerciseId = _userExerciseHistory.UserExerciseId,
			Weight = 33,
			Repetition = 33,
			UserId = _userExerciseHistory.UserId
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseHistory =
			await UserExerciseHistoriesRepository.Query().FirstOrDefaultAsync(x => x.Id == _userExerciseHistory.Id);


		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Value, Is.EqualTo(true));
			Assert.That(userExerciseHistory, Is.Not.Null);
			Assert.That(userExerciseHistory!.Id, Is.EqualTo(request.Id));
			Assert.That(userExerciseHistory.UserExerciseId, Is.EqualTo(request.UserExerciseId));
			Assert.That(userExerciseHistory.Weight, Is.EqualTo(request.Weight));
			Assert.That(userExerciseHistory.Repetition, Is.EqualTo(request.Repetition));
		});
	}

	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserIdIsInvalid()
	{
		// Arrange
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = _userExerciseHistory!.Id,
			Concurrency = _userExerciseHistory.Concurrency!,
			UserExerciseId = _userExerciseHistory.UserExerciseId,
			Weight = 33,
			Repetition = 33,
			UserId = "12345678-1234-1234-1234-123456789012"
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
		});
	}

	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserExerciseIdIsInvalid()
	{
		// Arrange
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = _userExerciseHistory!.Id,
			Concurrency = _userExerciseHistory.Concurrency!,
			UserExerciseId = 0,
			Weight = 33,
			Repetition = 33,
			UserId = _userExerciseHistory.UserId
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
		});
	}

	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenConcurrencyIsInvalid()
	{
		// Arrange
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = _userExerciseHistory!.Id,
			Concurrency = "",
			UserExerciseId = _userExerciseHistory.UserExerciseId,
			Weight = 33,
			Repetition = 33,
			UserId = _userExerciseHistory.UserId
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.ConcurrencyError));
		});
	}

	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserExerciseHistoryDoesNotExist()
	{
		// Arrange
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = 999, // Non-existent Id
			Concurrency = "some-concurrency-token",
			UserExerciseId = _userExerciseHistory!.UserExerciseId,
			Weight = 33,
			Repetition = 33,
			UserId = _userExerciseHistory.UserId
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
		});
	}

	[Test]
	public async Task UpdateUserExerciseHistoryCommandHandler_ShouldReturnError_WhenUserIsNotAuthorized()
	{
		// Arrange
		var request = new UpdateUserExerciseHistoryRequest
		{
			Id = _userExerciseHistory!.Id,
			Concurrency = _userExerciseHistory.Concurrency!,
			UserExerciseId = _userExerciseHistory.UserExerciseId,
			Weight = 33,
			Repetition = 33,
			UserId = _userExerciseHistory.UserId
		};
		var command = new UpdateUserExerciseHistoryCommand(request);

		// Mock the HttpContext to simulate an unauthorized user
		SetAnonymousUser();

		// Act
		var handler = new UpdateUserExerciseHistoryCommandHandler(UserExerciseHistoriesRepository, UsersRepository,
			UserExercisesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
			Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
		});
	}
}
