using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "MuscleFunction")]
public class UpdateMuscleFunctionCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleFunctions.AddRange(_muscleFunctions);
		DbContext.SaveChangesAsync();

		_handler = new UpdateMuscleFunctionCommandHandler(DbContext);
	}

	private readonly List<MuscleFunction> _muscleFunctions = ExerciseTypeData.MuscleFunctions().Take(3).ToList();
	private UpdateMuscleFunctionCommandHandler _handler;

	[Test]
	public async Task UpdateMuscleFunctionCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var storedMuscleFunction = await DbContext.MuscleFunctions.FindAsync(muscleFunction.Id);
		var updateRequest =
			new UpdateMuscleFunctionRequest(muscleFunction.Id, muscleFunction.Name, storedMuscleFunction?.Concurrency);
		var command = new UpdateMuscleFunctionCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedMuscleFunction = await DbContext.MuscleFunctions.FindAsync(muscleFunction.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedMuscleFunction, Is.Not.Null);
		Assert.That(updatedMuscleFunction.Id, Is.EqualTo(muscleFunction.Id));
		Assert.That(updatedMuscleFunction.Concurrency, Is.Not.Null);
		Assert.That(updatedMuscleFunction.Name, Is.EqualTo(muscleFunction.Name));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedMuscleFunction.CreatedAt,
			Is.EqualTo(muscleFunction.CreatedAt).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedMuscleFunction.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}

	[Test]
	public async Task UpdateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// - Create command
		var updateRequest = new UpdateMuscleFunctionRequest(1, "Name", Guid.NewGuid().ToString());
		var command = new UpdateMuscleFunctionCommand(updateRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenMuscleFunctionNotFound()
	{
		// Arrange
		var updateRequest = new UpdateMuscleFunctionRequest(5, "Name", Guid.NewGuid().ToString());
		var command = new UpdateMuscleFunctionCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UpdateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		var muscleFunction = _muscleFunctions.First();
		var updateRequest =
			new UpdateMuscleFunctionRequest(muscleFunction.Id, muscleFunction.Name, Guid.NewGuid().ToString());
		var command = new UpdateMuscleFunctionCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.ConcurrencyError));
	}

	[Test]
	public async Task UpdateMuscleFunctionCommandHandler_ShouldReturnErrorResult_WhenNamingConflict()
	{
		// Arrange
		var updateRequest = new UpdateMuscleFunctionRequest(
			_muscleFunctions.First().Id, _muscleFunctions.Last().Name, _muscleFunctions.First().Concurrency);
		var command = new UpdateMuscleFunctionCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}
}
