using FluentResults;
using Heracles.Application.Features.MuscleFunctions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.MuscleFunctions;

namespace Heracles.Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class CreateMuscleFunctionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleFunctions = MuscleFunctionsRepository.Query().ToList();
		_handler = new CreateMuscleFunctionCommandHandler(MuscleFunctionsRepository);
	}

	private List<MuscleFunction> _muscleFunctions;
	private CreateMuscleFunctionCommandHandler _handler;


	[Test]
	public async Task CreateMuscleFunctionCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateMuscleFunctionRequest("Unique Muscle Function Name");
		var command = new CreateMuscleFunctionCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var muscleFunction = await MuscleFunctionsRepository.GetByIdAsync(result.Value);
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
