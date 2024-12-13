using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunctions")]
public class RemoveMuscleFunctionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleFunctions.AddRange(_muscleFunctions);
		DbContext.SaveChangesAsync();

		_handler = new RemoveMuscleFunctionCommandHandler(DbContext);
	}

	private readonly List<MuscleFunction> _muscleFunctions = ExerciseTypeData.MuscleFunctions().Take(3).ToList();
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
