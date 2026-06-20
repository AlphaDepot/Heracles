using FluentResults;
using Heracles.Application.Features.MuscleFunctions.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class GetMuscleFunctionByIdHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleFunctions = MuscleFunctionsRepository.Query().ToList();
		_handler = new GetMuscleFunctionByIdQueryHandler(MuscleFunctionsRepository);
	}

	private List<MuscleFunction> _muscleFunctions;
	private GetMuscleFunctionByIdQueryHandler _handler;


	[Test]
	public async Task GetMuscleFunctionByIdHandler_ShouldReturnMuscleFunction()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var query = new GetMuscleFunctionByIdQuery(1);


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleFunction>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Name, Is.EqualTo(muscleFunction.Name));
	}


	[Test]
	public async Task GetMuscleFunctionByIdHandler_ShouldReturnErrorResult_WhenMuscleFunctionNotFound()
	{
		// Arrange
		var query = new GetMuscleFunctionByIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleFunction>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
