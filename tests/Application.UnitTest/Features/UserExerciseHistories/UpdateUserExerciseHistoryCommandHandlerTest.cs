using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExerciseHistories;
using Application.Features.UserExerciseHistories.Commands;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.UnitTest.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "UserExerciseHistory")]
public class UpdateUserExerciseHistoryCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.UserExerciseHistories.AddRange(_userExerciseHistories);
		DbContext.SaveChanges();
		_handler = new UpdateUserExerciseHistoryCommandHandler(DbContext, HttpContextAccessor);

		_userExerciseHistory =
			DbContext.UserExerciseHistories.FirstOrDefault(x => x.Id == _userExerciseHistories.First().Id);
		if (_userExerciseHistory == null)
		{
			throw new InvalidOperationException("UserExerciseHistory not found in the database");
		}
	}

	private readonly List<User> _users = UserData.Users();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private readonly List<UserExerciseHistory> _userExerciseHistories = UserExerciseData.UserExerciseHistories();
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
			await DbContext.UserExerciseHistories.FirstOrDefaultAsync(x => x.Id == _userExerciseHistory.Id);


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
			Assert.That(result.Value, Is.False);
			Assert.That(result.Error, Is.Not.Null);
			Assert.That(result.Error, Is.InstanceOf<Error>());
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
			Assert.That(result.Value, Is.False);
			Assert.That(result.Error, Is.Not.Null);
			Assert.That(result.Error, Is.InstanceOf<Error>());
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
			Assert.That(result.Value, Is.False);
			Assert.That(result.Error, Is.Not.Null);
			Assert.That(result.Error, Is.InstanceOf<Error>());
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
			Assert.That(result.Value, Is.False);
			Assert.That(result.Error, Is.Not.Null);
			Assert.That(result.Error, Is.InstanceOf<Error>());
			Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(UserExerciseHistory))));
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
		var unauthorizedUserId = "unauthorized-user-id";
		var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, unauthorizedUserId) };
		var identity = new ClaimsIdentity(claims, "TestAuthType");
		var claimsPrincipal = new ClaimsPrincipal(identity);
		HttpContextAccessor.HttpContext = new DefaultHttpContext { User = claimsPrincipal };

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<bool>>());
			Assert.That(result.Value, Is.False);
			Assert.That(result.Error, Is.Not.Null);
			Assert.That(result.Error, Is.InstanceOf<Error>());
			Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
		});
	}
}
