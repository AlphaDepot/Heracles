using FluentResults;
using Heracles.Application.Features.ExerciseMuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class RemoveExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseMuscleGroups = ExerciseMuscleGroupsRepository.Query().ToList();
		_handler = new RemoveExerciseMuscleGroupCommandHandler(ExerciseMuscleGroupsRepository);
	}

	private List<ExerciseMuscleGroup> _exerciseMuscleGroups;
	private RemoveExerciseMuscleGroupCommandHandler _handler;

	[Test]
	public async Task RemoveExerciseMuscleGroupCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveExerciseMuscleGroupCommand(1);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange
		// - Create command
		var command = new RemoveExerciseMuscleGroupCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		// - Create command
		var command = new RemoveExerciseMuscleGroupCommand(100, false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
