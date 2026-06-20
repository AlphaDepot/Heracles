using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class CreateExerciseTypeCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		_handler = new CreateExerciseTypeCommandHandler(ExerciseTypesRepository);
	}

	private List<ExerciseType> _exerciseTypes;
	private CreateExerciseTypeCommandHandler _handler;

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateExerciseTypeRequest("Unique Exercise Type Name", "Exercise Type Description",
			["Exercise Type Image Url"]);
		var command = new CreateExerciseTypeCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var exerciseType = await ExerciseTypesRepository.GetByIdAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(exerciseType, Is.Not.Null);
		Assert.That(exerciseType.Id, Is.EqualTo(result.Value));
		Assert.That(exerciseType.UpdatedAt, Is.InstanceOf<DateTime>());
		Assert.That(exerciseType.CreatedAt, Is.InstanceOf<DateTime>());
		Assert.That(exerciseType.Concurrency, Is.Not.Null);
		Assert.That(exerciseType.Name, Is.EqualTo(createRequest.Name));
		Assert.That(exerciseType.Description, Is.EqualTo(createRequest.Description));
		Assert.That(exerciseType.Images, Is.EqualTo(createRequest.Images));
	}

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		// - Seed data
		var exerciseType = _exerciseTypes.First();

		// - Create command
		var createRequest =
			new CreateExerciseTypeRequest(exerciseType.Name, exerciseType.Description, exerciseType.Images);
		var command = new CreateExerciseTypeCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
	}

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();

		var command = new CreateExerciseTypeCommand(
			new CreateExerciseTypeRequest(exerciseType.Name, exerciseType.Description, exerciseType.Images), false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
