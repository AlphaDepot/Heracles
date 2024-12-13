using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Commands;
using Application.Features.ExerciseTypes;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class CreateExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		_exerciseType.MuscleGroups = [];
		DbContext.ExerciseTypes.Add(_exerciseType);
		DbContext.MuscleGroups.Add(_muscleGroups);
		DbContext.MuscleFunctions.Add(_muscleFunctions);
		DbContext.SaveChanges();

		_handler = new CreateExerciseMuscleGroupCommandHandler(DbContext);
	}

	private readonly ExerciseType _exerciseType = ExerciseTypeData.ExerciseTypes().First();
	private readonly MuscleGroup _muscleGroups = ExerciseTypeData.MuscleGroups().First();
	private readonly MuscleFunction _muscleFunctions = ExerciseTypeData.MuscleFunctions().First();
	private CreateExerciseMuscleGroupCommandHandler _handler;


	[Test]
	public async Task CreateExerciseMuscleGroupCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateExerciseMuscleGroupRequest(1, 1, 1, 1);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var exerciseMuscleGroup = await DbContext.ExerciseMuscleGroups.FindAsync(result.Value);

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
		Assert.That(result.Error?.Type, Is.EqualTo(ErrorCodes.NotFound));
		Assert.That(result.Error, Is.EqualTo(
				ErrorTypes.NotFoundWithEntityName(nameof(MuscleGroup))
			)
		);
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
		Assert.That(result.Error?.Type, Is.EqualTo(ErrorCodes.NotFound));
		Assert.That(result.Error, Is.EqualTo(
				ErrorTypes.NotFoundWithEntityName(nameof(MuscleFunction))
			)
		);
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
		Assert.That(result.Error?.Type, Is.EqualTo(ErrorCodes.NotFound));
		Assert.That(result.Error, Is.EqualTo(
				ErrorTypes.NotFoundWithEntityName(nameof(ExerciseTypes))
			)
		);
	}


	[Test]
	public async Task
		CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupIsDuplicate()
	{
		// Arrange
		// - Seed data
		var exerciseType = DbContext.ExerciseTypes.First(x => x.Id == _exerciseType.Id);
		var muscleGroup = DbContext.MuscleGroups.First(x => x.Id == _muscleGroups.Id);
		var muscleFunction = DbContext.MuscleFunctions.First(x => x.Id == _muscleFunctions.Id);
		exerciseType.MuscleGroups = new List<ExerciseMuscleGroup>
		{
			new()
			{
				Muscle = muscleGroup,
				Function = muscleFunction
			}
		};
		await DbContext.SaveChangesAsync();

		// - Create command
		var createRequest = new CreateExerciseMuscleGroupRequest(
			_exerciseType.Id, _exerciseType.MuscleGroups!.First().Muscle.Id,
			_exerciseType.MuscleGroups!.First().Function.Id, 50.6);
		var command = new CreateExerciseMuscleGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		// validate only the error type because the message may change
		Assert.That(result.Error, Is.EqualTo(
				ErrorTypes.DuplicateEntryWithEntityNames(
					nameof(ExerciseMuscleGroup),
					nameof(ExerciseTypes)
				)
			)
		);
	}

	[Test]
	public async Task
		CreateExerciseMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		// no need to seed since the user is not admin and validation for user is handle before the entity validation

		var command = new CreateExerciseMuscleGroupCommand(
			new CreateExerciseMuscleGroupRequest(1, 1, 1, 50.6), false);
		var handler = new CreateExerciseMuscleGroupCommandHandler(DbContext);

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
