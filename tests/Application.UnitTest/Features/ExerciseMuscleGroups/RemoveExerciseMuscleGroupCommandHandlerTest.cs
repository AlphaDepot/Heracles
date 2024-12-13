using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class RemoveExerciseMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.ExerciseMuscleGroups.AddRange(_exerciseMuscleGroups);
		DbContext.SaveChanges();

		_handler = new RemoveExerciseMuscleGroupCommandHandler(DbContext);
	}

	private readonly List<ExerciseMuscleGroup> _exerciseMuscleGroups = ExerciseTypeData.ExerciseMuscleGroups();

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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
