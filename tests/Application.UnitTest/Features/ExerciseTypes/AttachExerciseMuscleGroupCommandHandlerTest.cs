using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class AttachExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();

		_handler = new AttachExerciseMuscleGroupCommandHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private AttachExerciseMuscleGroupCommandHandler _handler;


	[Test]
	public async Task AttachExerciseMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var exerciseMuscleGroup = ExerciseTypeData.ExerciseMuscleGroups().Last();
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
		var exerciseMuscleGroup = ExerciseTypeData.ExerciseMuscleGroups().First();
		var request = new AttachExerciseMuscleGroupRequest(1000, exerciseMuscleGroup.Id);
		var addRequest = new AttachExerciseMuscleGroupCommand(request);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}
}
