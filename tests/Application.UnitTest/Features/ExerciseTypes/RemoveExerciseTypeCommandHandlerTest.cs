using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class RemoveExerciseTypeCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();

		_handler = new RemoveExerciseTypeCommandHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private RemoveExerciseTypeCommandHandler _handler;

	[Test]
	public async Task RemoveExerciseTypeCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveExerciseTypeCommand(1);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenExerciseTypeNotFound()
	{
		// Arrange
		var command = new RemoveExerciseTypeCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveExerciseTypeCommand(100, false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
