using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class DetachExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.QueryTracking().ToList();
		_handler = new DetachExerciseMuscleGroupCommandHandler(ExerciseTypesRepository, ExerciseMuscleGroupsRepository);
	}

	private List<ExerciseType> _exerciseTypes;
	private DetachExerciseMuscleGroupCommandHandler _handler;


	[Test]
	public async Task DetachExerciseMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var exerciseMuscleGroup = exerciseType.MuscleGroups!.First();
		var request = new DetachExerciseMuscleGroupRequest(exerciseType.Id, exerciseMuscleGroup.Id);
		var detachRequest = new DetachExerciseMuscleGroupCommand(request);


		// Act
		var result = await _handler.Handle(detachRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task
		DetachExerciseMuscleGroupCommandHandler_ShouldReturnNotFoundErrorResult__WhenExerciseTypeNotFound()
	{
		// Arrange
		var request = new DetachExerciseMuscleGroupRequest(1000, 1);
		var detachRequest = new DetachExerciseMuscleGroupCommand(request);


		// Act
		var result = await _handler.Handle(detachRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}


	[Test]
	public async Task
		DetachExerciseMuscleGroupCommandHandler_ShouldReturnInvalidRequestErrorResult_WhenExerciseMuscleGroupNotAttached()
	{
		// Arrange
		var request =
			new DetachExerciseMuscleGroupRequest(_exerciseTypes.First().Id, _exerciseTypes[2].MuscleGroups!.First().Id);
		var detachRequest =
			new DetachExerciseMuscleGroupCommand(request);


		// Act
		var result = await _handler.Handle(detachRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.BadRequest));
	}

	[Test]
	public async Task
		DetachExerciseMuscleGroupCommandHandler_ShouldReturnNotFoundErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange
		var request = new DetachExerciseMuscleGroupRequest(_exerciseTypes.First().Id, 1000);
		var detachRequest = new DetachExerciseMuscleGroupCommand(request);


		// Act
		var result = await _handler.Handle(detachRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
