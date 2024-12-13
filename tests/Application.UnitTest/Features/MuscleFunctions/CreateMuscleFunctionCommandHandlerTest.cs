using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class CreateMuscleFunctionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleFunctions.AddRange(_muscleFunctions);
		DbContext.SaveChangesAsync();

		_handler = new CreateMuscleFunctionCommandHandler(DbContext);
	}

	private readonly List<MuscleFunction> _muscleFunctions = ExerciseTypeData.MuscleFunctions();
	private CreateMuscleFunctionCommandHandler _handler;


	[Test]
	public async Task CreateMuscleFunctionCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateMuscleFunctionRequest("Unique Muscle Function Name");
		var command = new CreateMuscleFunctionCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var muscleFunction = await DbContext.MuscleFunctions.FindAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(muscleFunction, Is.Not.Null);
		Assert.That(muscleFunction.Id, Is.EqualTo(result.Value));
		Assert.That(muscleFunction.CreatedAt, Is.TypeOf<DateTime>());
		Assert.That(muscleFunction.UpdatedAt, Is.TypeOf<DateTime>());
		Assert.That(muscleFunction.Concurrency, Is.Not.Null);
		Assert.That(muscleFunction.Name, Is.EqualTo(createRequest.Name));
	}

	[Test]
	public async Task CreateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var createRequest = new CreateMuscleFunctionRequest(muscleFunction.Name);
		var command = new CreateMuscleFunctionCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}

	[Test]
	public async Task CreateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var command = new CreateMuscleFunctionCommand(
			new CreateMuscleFunctionRequest(muscleFunction.Name), false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
