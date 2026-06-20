using FluentResults;
using Heracles.Application.Features.MuscleFunctions.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class GetPagedMuscleFunctionsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleFunctions = MuscleFunctionsRepository.Query().ToList();
		_handler = new GetPagedMuscleFunctionsQueryHandler(MuscleFunctionsRepository);
	}

	private List<MuscleFunction> _muscleFunctions;
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
		Assert.That(result.Value.Data.Count, Is.EqualTo(_muscleFunctions.Count));
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
