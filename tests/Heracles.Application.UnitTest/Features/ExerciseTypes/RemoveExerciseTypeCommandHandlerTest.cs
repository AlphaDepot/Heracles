using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class RemoveExerciseTypeCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		_handler = new RemoveExerciseTypeCommandHandler(ExerciseTypesRepository);
	}

	private List<ExerciseType> _exerciseTypes;
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
