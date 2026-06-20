using FluentResults;
using Heracles.Application.Features.UserExercises.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.UserExercises;

namespace Heracles.Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class CreateUserExerciseCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.Query().ToList();
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		// Seed data


		_createRequest = new CreateUserExerciseRequest
		{
			UserId = _users.First().UserId,
			ExerciseTypeId = _exerciseTypes.First().Id,
			StaticResistance = 1,
			PercentageResistance = 1,
			CurrentWeight = 1,
			PersonalRecord = 1,
			DurationInSeconds = 1,
			SortOrder = 1,
			Repetitions = 1,
			Sets = 1,
			Timed = true,
			BodyWeight = true
		};

		_handler = new CreateUserExerciseCommandHandler(UserExercisesRepository,
			UsersRepository,
			ExerciseTypesRepository,
			CurrentUserServiceMock);
	}

	private List<User> _users;
	private List<ExerciseType> _exerciseTypes;
	private CreateUserExerciseRequest _createRequest;
	private CreateUserExerciseCommandHandler _handler;


	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExercise = await UserExercisesRepository.GetByIdAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(userExercise, Is.Not.Null);
		Assert.That(userExercise.Id, Is.EqualTo(result.Value));
	}

	[Test]
	public async Task
		CreateUserExerciseCommandHandler_ShouldReturnIntId_AndVersionSetTo2_WhenUserExerciseAlreadyExists()
	{
		// Arrange
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var result2 = await _handler.Handle(command, CancellationToken.None);
		var newUserExercise = await UserExercisesRepository.GetByIdAsync(result2.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(result2, Is.Not.Null);
		Assert.That(result2, Is.InstanceOf<Result<int>>());
		Assert.That(result2.Value, Is.GreaterThan(0));

		Assert.That(newUserExercise, Is.Not.Null);
		Assert.That(newUserExercise.Id, Is.EqualTo(result2.Value));
		Assert.That(newUserExercise.Version, Is.EqualTo(2));
	}


	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserIdIsInvalid()
	{
		// Arrange
		_createRequest.UserId = "Invalid User Id";
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		Console.WriteLine(_createRequest.UserId);
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnErrorResult_WhenExerciseTypeIdIsInvalid()
	{
		// Arrange
		_createRequest.ExerciseTypeId = 0;
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		var user = _users.First(x => x.UserId != CurrentUserServiceMock.UserId);
		_createRequest.UserId = user.UserId;
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
