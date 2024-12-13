using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Commands;
using Application.Features.ExerciseTypes;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class UpdateExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChanges();
		_handler = new UpdateExerciseMuscleGroupCommandHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private readonly List<ExerciseMuscleGroup> _exerciseMuscleGroups = ExerciseTypeData.ExerciseMuscleGroups();
	private UpdateExerciseMuscleGroupCommandHandler _handler;

	[Test]
	public async Task UpdateExerciseMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var storedExerciseMuscleGroup =
			await DbContext.ExerciseMuscleGroups.FindAsync(_exerciseTypes.First().MuscleGroups!.First().Id);
		var updateRequest = new UpdateExerciseMuscleGroupRequest(storedExerciseMuscleGroup!.Id,
			storedExerciseMuscleGroup.Concurrency, storedExerciseMuscleGroup.FunctionPercentage);
		var command = new UpdateExerciseMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedExerciseMuscleGroup = await DbContext.ExerciseMuscleGroups.FindAsync(storedExerciseMuscleGroup.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedExerciseMuscleGroup, Is.Not.Null);
		Assert.That(updatedExerciseMuscleGroup.Id, Is.EqualTo(storedExerciseMuscleGroup.Id));
		Assert.That(updatedExerciseMuscleGroup.Concurrency, Is.Not.Null);
		Assert.That(updatedExerciseMuscleGroup.FunctionPercentage,
			Is.EqualTo(storedExerciseMuscleGroup.FunctionPercentage));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedExerciseMuscleGroup.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedExerciseMuscleGroup.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}


	[Test]
	public async Task UpdateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var updateRequest = new UpdateExerciseMuscleGroupRequest(1, Guid.NewGuid().ToString(), 1);
		var command = new UpdateExerciseMuscleGroupCommand(updateRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange\
		// Note - ExerciseMuscleGroup with Id 1000 does not exist
		var updateRequest = new UpdateExerciseMuscleGroupRequest(1000, Guid.NewGuid().ToString(), 1);
		var command = new UpdateExerciseMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UpdateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		var updateRequest =
			new UpdateExerciseMuscleGroupRequest(_exerciseMuscleGroups.First().Id, Guid.NewGuid().ToString(),
				_exerciseMuscleGroups.First().FunctionPercentage);
		var command = new UpdateExerciseMuscleGroupCommand(updateRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.ConcurrencyError));
	}
}
