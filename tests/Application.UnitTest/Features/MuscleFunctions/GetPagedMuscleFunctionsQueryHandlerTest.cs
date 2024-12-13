using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class GetPagedMuscleFunctionsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleFunctions.AddRange(_muscleFunctions);
		DbContext.SaveChangesAsync();

		_handler = new GetPagedMuscleFunctionsQueryHandler(DbContext);
	}

	private readonly List<MuscleFunction> _muscleFunctions = ExerciseTypeData.MuscleFunctions().Take(3).ToList();
	private GetPagedMuscleFunctionsQueryHandler _handler;


	[Test]
	public async Task GetMuscleFunctionsQueryHandler_ShouldReturnPagedMuscleFunctions()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var query = new GetPagedMuscleFunctionsQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<MuscleFunction>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(3));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(muscleFunction.Name));
	}


	[Test]
	public async Task GetMuscleFunctionsQueryHandler_ShouldReturnPagedMuscleFunctionsWithSearchTerm()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var query = new GetPagedMuscleFunctionsQuery(new QueryRequest { SearchTerm = muscleFunction.Name });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<MuscleFunction>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(muscleFunction.Name));
	}
}
