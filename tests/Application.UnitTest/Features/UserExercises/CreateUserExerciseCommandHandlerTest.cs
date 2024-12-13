using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.UserExercises.Commands;
using Application.Features.Users;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class CreateUserExerciseCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		var users = UserData.Users();
		var exerciseTypes = ExerciseTypeData.ExerciseTypes();
		// Seed data
		DbContext.Users.AddRange(users);
		DbContext.ExerciseTypes.AddRange(exerciseTypes);
		DbContext.SaveChanges();

		_createRequest = new CreateUserExerciseRequest
		{
			UserId = users.First().UserId,
			ExerciseTypeId = exerciseTypes.First().Id,
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

		_handler = new CreateUserExerciseCommandHandler(DbContext, HttpContextAccessor);
	}

	private CreateUserExerciseRequest _createRequest;

	private CreateUserExerciseCommandHandler _handler;


	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExercise = await DbContext.UserExercises.FindAsync(result.Value);

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
		var newUserExercise = await DbContext.UserExercises.FindAsync(result2.Value);

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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(User))));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFoundWithEntityName(nameof(ExerciseType))));
	}

	[Test]
	public async Task CreateUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDifferentFromCurrentUserId()
	{
		// Arrange
		var user = UserData.Users()[2];
		_createRequest.UserId = user.UserId;
		var command = new CreateUserExerciseCommand(_createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
