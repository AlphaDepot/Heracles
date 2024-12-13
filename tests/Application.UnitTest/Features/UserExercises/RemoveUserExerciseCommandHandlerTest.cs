using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.UserExercises.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class RemoveUserExerciseCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new RemoveUserExerciseCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private RemoveUserExerciseCommandHandler _handler;

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var userExercise = _userExercises.First();
		// - Create command
		var command = new RemoveUserExerciseCommand(userExercise.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseRemoved = await DbContext.UserExercises.FindAsync(userExercise.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(userExerciseRemoved, Is.Null);
	}

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var command = new RemoveUserExerciseCommand(10000);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		var userExercise = _userExercises.Last();
		var command = new RemoveUserExerciseCommand(userExercise.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
