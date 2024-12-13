using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class GetPagedExerciseTypesQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();

		_handler = new GetPagedExerciseTypesQueryHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes().Take(3).ToList();
	private GetPagedExerciseTypesQueryHandler _handler;

	[Test]
	public async Task GetPagedExerciseTypesQueryHandler_ShouldReturnPagedExerciseTypes()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		// - Create query
		var query = new GetPagedExerciseTypesQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseType>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(3));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(exerciseType.Name));
		Assert.That(result.Value.Data.First().Description, Is.EqualTo(exerciseType.Description));
		Assert.That(result.Value.Data.First().ImageUrl, Is.EqualTo(exerciseType.ImageUrl));
	}


	[Test]
	public async Task GetPagedExerciseTypesQueryHandler_ShouldReturnPagedExerciseTypesWithSearchTerm()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var query = new GetPagedExerciseTypesQuery(new QueryRequest { SearchTerm = exerciseType.Name });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseType>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(exerciseType.Name));
		Assert.That(result.Value.Data.First().Description, Is.EqualTo(exerciseType.Description));
		Assert.That(result.Value.Data.First().ImageUrl, Is.EqualTo(exerciseType.ImageUrl));
	}

	[Test]
	public async Task GetPagedExerciseTypesQueryHandler_ShouldReturnPagedExerciseTypesWithSort_ByNameDecending()
	{
		// Arrange
		var sortedExerciseTypes = _exerciseTypes.OrderBy(et => et.Name).ToList();
		var query = new GetPagedExerciseTypesQuery(new QueryRequest { SortBy = "name" });


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseType>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(_exerciseTypes.Count));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(sortedExerciseTypes.First().Id));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(sortedExerciseTypes.First().Name));
		Assert.That(result.Value.Data.First().Description, Is.EqualTo(sortedExerciseTypes.First().Description));
		Assert.That(result.Value.Data.First().ImageUrl, Is.EqualTo(sortedExerciseTypes.First().ImageUrl));
	}
}
