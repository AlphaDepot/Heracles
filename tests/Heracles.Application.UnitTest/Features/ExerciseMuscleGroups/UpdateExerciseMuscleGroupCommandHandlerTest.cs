using FluentResults;
using Heracles.Application.Features.ExerciseMuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.ExerciseMuscleGroups;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class UpdateExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.QueryTracking().ToList();
		_exerciseMuscleGroups = ExerciseMuscleGroupsRepository.QueryTracking().ToList();
		_handler = new UpdateExerciseMuscleGroupCommandHandler(ExerciseMuscleGroupsRepository);
	}

	private List<ExerciseType> _exerciseTypes;
	private List<ExerciseMuscleGroup> _exerciseMuscleGroups;
	private UpdateExerciseMuscleGroupCommandHandler _handler;

	[Test]
	public async Task UpdateExerciseMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var storedExerciseMuscleGroup =
			ExerciseMuscleGroupsRepository.QueryTracking()
				.Include(x => x.Function)
				.Include(x => x.Muscle)
				.FirstOrDefault(x => x.Id == _exerciseTypes.First().MuscleGroups!.First().Id);
		var updateRequest = new UpdateExerciseMuscleGroupRequest(storedExerciseMuscleGroup!.Id,
			storedExerciseMuscleGroup.Concurrency, storedExerciseMuscleGroup.FunctionPercentage);
		var command = new UpdateExerciseMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedExerciseMuscleGroup = ExerciseMuscleGroupsRepository.QueryTracking()
			.Include(x => x.Function)
			.Include(x => x.Muscle)
			.FirstOrDefault(x => x.Id == storedExerciseMuscleGroup.Id);

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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.ConcurrencyError));
	}
}
