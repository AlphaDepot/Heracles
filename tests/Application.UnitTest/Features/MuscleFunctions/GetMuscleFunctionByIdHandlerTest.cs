using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class GetMuscleFunctionByIdHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleFunctions.AddRange(_muscleFunctions);
		DbContext.SaveChangesAsync();

		_handler = new GetMuscleFunctionByIdQueryHandler(DbContext);
	}

	private readonly List<MuscleFunction> _muscleFunctions = ExerciseTypeData.MuscleFunctions();
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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}
}
