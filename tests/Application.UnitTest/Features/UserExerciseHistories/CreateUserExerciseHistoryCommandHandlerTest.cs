using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.UserExerciseHistories.Commands;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExerciseHistories;
[TestFixture(Category = "UserExerciseHistory")]
public class CreateUserExerciseHistoryCommandHandlerTest: HandlerBaseUnitTest
{
	private readonly List<User> _users = UserData.Users();
	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private CreateUserExerciseHistoryCommandHandler _handler;


	[SetUp]
	public void SetUp()
	{
		DbContext.Users.AddRange(_users);
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();
		_handler = new CreateUserExerciseHistoryCommandHandler(DbContext, HttpContextAccessor);
	}

	[Test]
	public async Task CreateUserExerciseHistoryCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateUserExerciseHistoryRequest(_userExercises.First().Id, 10, 10, _users.First().UserId);
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseHistory = await DbContext.UserExerciseHistories.FindAsync(result.Value);

		Console.WriteLine(result.Error);

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
		var request = new CreateUserExerciseHistoryRequest(_userExercises.First().Id, 10, 10, "12345678-1234-1234-1234-123456789012");
		var command = new CreateUserExerciseHistoryCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.InstanceOf<Result<int>>());
			Assert.That(result.IsFailure, Is.True);
			Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(User))));
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
			Assert.That(result.IsFailure, Is.True);
			Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(UserExercise))));
		});
	}

	[Test]
	public async Task CreateUserExerciseHistoryCommandHandler_ShouldReturnError__WhenUserIdIsDifferentFromCurrentUserId()
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
			Assert.That(result.IsFailure, Is.True);
			Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
		});
	}


}
