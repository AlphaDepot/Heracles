using FluentResults;
using Heracles.Application.Features.MuscleFunctions.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class RemoveMuscleFunctionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleFunctions = MuscleFunctionsRepository.Query().ToList();
		_handler = new RemoveMuscleFunctionCommandHandler(MuscleFunctionsRepository);
	}

	private List<MuscleFunction> _muscleFunctions;
	private RemoveMuscleFunctionCommandHandler _handler;

	[Test]
	public async Task RemoveMuscleFunctionCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveMuscleFunctionCommand(1);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenMuscleFunctionNotFound()
	{
		// Arrange
		var command = new RemoveMuscleFunctionCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveMuscleFunctionCommand(100, false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
