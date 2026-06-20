using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class GetPagedExerciseTypesQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		_handler = new GetPagedExerciseTypesQueryHandler(ExerciseTypesRepository);
	}

	private List<ExerciseType> _exerciseTypes;
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
		Assert.That(result.Value.Data.Count, Is.EqualTo(_exerciseTypes.Count));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(exerciseType.Name));
		Assert.That(result.Value.Data.First().Description, Is.EqualTo(exerciseType.Description));
		Assert.That(result.Value.Data.First().Images, Is.EqualTo(exerciseType.Images));
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
		Assert.That(result.Value.Data.First().Images, Is.EqualTo(exerciseType.Images));
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
		Assert.That(result.Value.Data.First().Images, Is.EqualTo(sortedExerciseTypes.First().Images));
	}
}
