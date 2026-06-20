using FluentResults;
using Heracles.Application.Features.ExerciseMuscleGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class GetExerciseMuscleGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_handler = new GetExerciseMuscleGroupByIdQueryHandler(ExerciseMuscleGroupsRepository);
	}

	private GetExerciseMuscleGroupByIdQueryHandler _handler;

	[Test]
	public async Task GetExerciseMuscleGroupByIdQueryHandler_ShouldReturnExerciseMuscleGroup()
	{
		// Arrange
		// - Create query
		var query = new GetExerciseMuscleGroupByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseMuscleGroup>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.ExerciseTypeId, Is.EqualTo(1));
	}


	[Test]
	public async Task GetExerciseMuscleGroupByIdQueryHandler_ShouldReturnErrorResult_WhenExerciseMuscleGroupNotFound()
	{
		// Arrange
		var query = new GetExerciseMuscleGroupByIdQuery(1000);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseMuscleGroup>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
