using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class GetExerciseMuscleGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_handler = new GetExerciseMuscleGroupByIdQueryHandler(DbContext);
	}

	private GetExerciseMuscleGroupByIdQueryHandler _handler;

	[Test]
	public async Task GetExerciseMuscleGroupByIdQueryHandler_ShouldReturnExerciseMuscleGroup()
	{
		// Arrange
		// - Seed data
		var exerciseMuscleGroup = ExerciseTypeData.ExerciseMuscleGroups().First();
		DbContext.ExerciseMuscleGroups.Add(exerciseMuscleGroup);
		await DbContext.SaveChangesAsync();
		// - Create query
		var query = new GetExerciseMuscleGroupByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseMuscleGroup>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.ExerciseTypeId, Is.EqualTo(exerciseMuscleGroup.ExerciseTypeId));
	}


	[Test]
	public async Task GetExerciseMuscleGroupByIdQueryHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange
		var query = new GetExerciseMuscleGroupByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseMuscleGroup>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}
}
