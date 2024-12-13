using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class GetMuscleGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleGroups.AddRange(_muscleGroups);
		DbContext.SaveChangesAsync();

		_handler = new GetMuscleGroupByIdQueryHandler(DbContext);
	}

	private readonly List<MuscleGroup> _muscleGroups = ExerciseTypeData.MuscleGroups().Take(3).ToList();
	private GetMuscleGroupByIdQueryHandler _handler;


	[Test]
	public async Task GetMuscleGroupByIdQueryHandler_ShouldReturnMuscleGroup()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var query = new GetMuscleGroupByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleGroup>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Name, Is.EqualTo(muscleGroup.Name));
	}


	[Test]
	public async Task GetMuscleGroupByIdQueryHandler_ShouldReturnErrorResult_WhenMuscleGroupNotFound()
	{
		// Arrange
		var query = new GetMuscleGroupByIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleGroup>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}
}
