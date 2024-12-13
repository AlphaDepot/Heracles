using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.UserExercises.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class UpdateUserExerciseCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new UpdateUserExerciseCommandHandler(DbContext, HttpContextAccessor);


		_updateRequest = new UpdateUserExerciseRequest
		{
			Id = _userExercises.First().Id,
			Concurrency = _userExercises.First().Concurrency ?? Guid.NewGuid().ToString(),
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
	}

	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();

	private UpdateUserExerciseRequest _updateRequest;
	private UpdateUserExerciseCommandHandler _handler;

	[Test]
	public async Task UpdateUserExerciseCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var command = new UpdateUserExerciseCommand(_updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExercise = await DbContext.UserExercises.FindAsync(_updateRequest.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(userExercise, Is.Not.Null);
		Assert.That(userExercise.Id, Is.EqualTo(_updateRequest.Id));
		Assert.That(userExercise.StaticResistance, Is.EqualTo(_updateRequest.StaticResistance));
		Assert.That(userExercise.BodyWeight, Is.EqualTo(_updateRequest.BodyWeight));
	}

	[Test]
	public async Task UpdateUserExerciseCommandHandler_ShouldReturnFailureResult_WhenUserExerciseDoesNotExist()
	{
		// Arrange
		_updateRequest.Id = 999;
		var command = new UpdateUserExerciseCommand(_updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UpdateUserExerciseCommandHandler_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
	{
		// Arrange
		var userExercise = _userExercises.Last();
		_updateRequest.Id = userExercise.Id;
		var command = new UpdateUserExerciseCommand(_updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateUserExerciseCommandHandler_ShouldReturnConcurrencyError_WhenConcurrencyError()
	{
		// Arrange
		_updateRequest.Concurrency = Guid.NewGuid().ToString();
		var command = new UpdateUserExerciseCommand(_updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.ConcurrencyError));
	}
}
