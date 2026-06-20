using System.Net;
using FluentResults;
using Heracles.Application.Features.ExerciseMuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.ExerciseMuscleGroups;

namespace Heracles.Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class CreateExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseType = ExerciseTypesRepository.Query().First();
		_muscleGroups = MuscleGroupsRepository.Query().First();
		_muscleFunctions = MuscleFunctionsRepository.Query().First();

		_handler = new CreateExerciseMuscleGroupCommandHandler(
			ExerciseMuscleGroupsRepository,
			MuscleGroupsRepository,
			MuscleFunctionsRepository,
			ExerciseTypesRepository
		);
	}

	private ExerciseType _exerciseType;
	private MuscleGroup _muscleGroups;
	private MuscleFunction _muscleFunctions;
	private CreateExerciseMuscleGroupCommandHandler _handler;


	[Test]
	public async Task CreateExerciseMuscleGroupCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		// Add a new exercise to guarantee initial empty associations
		await ExerciseTypesRepository.AddAsync(new ExerciseType { Name = "Test.Exercise.Type" });
		await ExerciseTypesRepository.SaveChangesAsync();

		_exerciseType = ExerciseTypesRepository.Query().First(x => x.Name == "Test.Exercise.Type");
		_muscleGroups = MuscleGroupsRepository.Query().First();
		_muscleFunctions = MuscleFunctionsRepository.Query().First();

		var createRequest =
			new CreateExerciseMuscleGroupRequest(_exerciseType.Id, _muscleGroups.Id, _muscleFunctions.Id, 1);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		result.LogFailedResult();
		var exerciseMuscleGroup = await ExerciseMuscleGroupsRepository.GetByIdAsync(result.Value);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(exerciseMuscleGroup, Is.Not.Null);
		Assert.That(exerciseMuscleGroup.Id, Is.EqualTo(result.Value));
		Assert.That(exerciseMuscleGroup.CreatedAt, Is.TypeOf<DateTime>());
		Assert.That(exerciseMuscleGroup.UpdatedAt, Is.TypeOf<DateTime>());
		Assert.That(exerciseMuscleGroup.Concurrency, Is.Not.Null);
		Assert.That(exerciseMuscleGroup.ExerciseTypeId, Is.EqualTo(createRequest.ExerciseTypeId));
	}


	[Test]
	public async Task CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenMuscleGroupIsNotFound()
	{
		// Arrange
		// note make sure id is not in the seed data to avoid conflict, currently the id is 1000
		var createRequest = new CreateExerciseMuscleGroupRequest(_exerciseType.Id, 1000, _muscleFunctions.Id, 50.6);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["StatusCode"], Is.EqualTo((int)HttpStatusCode.NotFound));
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenMuscleFunctionIsNotFound()
	{
		// Arrange
		// note make sure id is not in the seed data to avoid conflict, currently the id is 1000
		var createRequest = new CreateExerciseMuscleGroupRequest(_exerciseType.Id, _muscleGroups.Id, 100, 50.6);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["StatusCode"], Is.EqualTo((int)HttpStatusCode.NotFound));
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenExerciseTypeIsNotFound()
	{
		// Arrange
		// note make sure id is not in the seed data to avoid conflict, currently the id is 1000
		var createRequest = new CreateExerciseMuscleGroupRequest(1000, _muscleGroups.Id, _muscleFunctions.Id, 50.6);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["StatusCode"], Is.EqualTo((int)HttpStatusCode.NotFound));
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound)
		);
	}


	[Test]
	public async Task
		CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupIsDuplicate()
	{
		// Arrange
		var exerciseType = await ExerciseTypesRepository.GetByIdAsync(_exerciseType.Id);
		var muscleGroup = await MuscleGroupsRepository.GetByIdAsync(_muscleGroups.Id);
		var muscleFunction = await MuscleFunctionsRepository.GetByIdAsync(_muscleFunctions.Id);

		exerciseType.MuscleGroups = new List<ExerciseMuscleGroup>
		{
			new()
			{
				ExerciseTypeId = exerciseType.Id,
				Muscle = muscleGroup,
				MuscleId = muscleGroup.Id,
				Function = muscleFunction,
				FunctionId = muscleFunction.Id,
				FunctionPercentage = 100
			}
		};

		await ExerciseMuscleGroupsRepository.AddAsync(new ExerciseMuscleGroup
		{
			ExerciseTypeId = exerciseType.Id,
			Muscle = muscleGroup,
			MuscleId = muscleGroup.Id,
			Function = muscleFunction,
			FunctionId = muscleFunction.Id,
			FunctionPercentage = 50.6
		});

		await ExerciseMuscleGroupsRepository.SaveChangesAsync();

		// Act
		var createRequest = new CreateExerciseMuscleGroupRequest(
			exerciseType.Id,
			muscleGroup.Id,
			muscleFunction.Id,
			50.6);

		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.DuplicateEntry));
	}

	[Test]
	public async Task
		CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		// no need to seed since the user is not admin and validation for user is handle before the entity validation

		var command = new CreateExerciseMuscleGroupCommand(
			new CreateExerciseMuscleGroupRequest(1, 1, 1, 50.6), false);
		var handler = new CreateExerciseMuscleGroupCommandHandler(
			ExerciseMuscleGroupsRepository,
			MuscleGroupsRepository,
			MuscleFunctionsRepository,
			ExerciseTypesRepository
		);


		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
