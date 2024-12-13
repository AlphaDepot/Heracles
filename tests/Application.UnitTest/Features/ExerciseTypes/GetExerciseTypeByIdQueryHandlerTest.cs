using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class GetExerciseTypeByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();

		_handler = new GetExerciseTypeByIdQueryHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private GetExerciseTypeByIdQueryHandler _handler;


	[Test]
	public async Task GetExerciseTypeByIdQueryHandler_ShouldReturnExerciseType()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var query = new GetExerciseTypeByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseType>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Name, Is.EqualTo(exerciseType.Name));
		Assert.That(result.Value.Description, Is.EqualTo(exerciseType.Description));
		Assert.That(result.Value.ImageUrl, Is.EqualTo(exerciseType.ImageUrl));
	}

	[Test]
	public async Task GetExerciseTypeByIdQueryHandler_ShouldReturnErrorResult_WhenExerciseTypeNotFound()
	{
		// Arrange
		var query = new GetExerciseTypeByIdQuery(1000);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseType>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}
}
