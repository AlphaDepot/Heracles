using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class AttachExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		_handler = new AttachExerciseMuscleGroupCommandHandler(ExerciseTypesRepository, ExerciseMuscleGroupsRepository);
	}

	private List<ExerciseType> _exerciseTypes;
	private AttachExerciseMuscleGroupCommandHandler _handler;


	[Test]
	public async Task AttachExerciseMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var exerciseMuscleGroup = ExerciseMuscleGroupsRepository.Query().ToList().Last();
		var request = new AttachExerciseMuscleGroupRequest(exerciseType.Id, exerciseMuscleGroup.Id);
		var addRequest = new AttachExerciseMuscleGroupCommand(request);


		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task AttachExerciseMuscleGroupCommandHandler_ShouldReturnNotFoundErrorResult_WhenExerciseTypeNotFound()
	{
		// Arrange
		var exerciseMuscleGroup = ExerciseMuscleGroupsRepository.Query().First();
		var request = new AttachExerciseMuscleGroupRequest(1000, exerciseMuscleGroup.Id);
		var addRequest = new AttachExerciseMuscleGroupCommand(request);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task
		AttachExerciseMuscleGroupCommandHandler_ShouldReturnNotFoundErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var request = new AttachExerciseMuscleGroupRequest(exerciseType.Id, 1000);
		var addRequest = new AttachExerciseMuscleGroupCommand(request);
		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
