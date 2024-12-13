using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class UpdateExerciseTypeCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();

		_handler = new UpdateExerciseTypeCommandHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private UpdateExerciseTypeCommandHandler _handler;

	[Test]
	public async Task UpdateExerciseTypeCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var storedExerciseType = await DbContext.ExerciseTypes.FindAsync(exerciseType.Id);
		var updateRequest = new UpdateExerciseTypeRequest(exerciseType.Id, exerciseType.Name,
			storedExerciseType?.Concurrency, exerciseType.Description, exerciseType.ImageUrl);
		var command = new UpdateExerciseTypeCommand(updateRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedExerciseType = await DbContext.ExerciseTypes.FindAsync(exerciseType.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedExerciseType, Is.Not.Null);
		Assert.That(updatedExerciseType.Id, Is.EqualTo(exerciseType.Id));
		Assert.That(updatedExerciseType.Concurrency, Is.Not.Null);
		Assert.That(updatedExerciseType.Name, Is.EqualTo(exerciseType.Name));
		Assert.That(updatedExerciseType.Description, Is.EqualTo(exerciseType.Description));
		Assert.That(updatedExerciseType.ImageUrl, Is.EqualTo(exerciseType.ImageUrl));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedExerciseType.CreatedAt, Is.EqualTo(exerciseType.CreatedAt).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedExerciseType.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}


	[Test]
	public async Task UpdateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var updateRequest =
			new UpdateExerciseTypeRequest(1, "Name", Guid.NewGuid().ToString(), "Description", "ImageUrl");
		var command = new UpdateExerciseTypeCommand(updateRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenExerciseTypeNotFound()
	{
		// Arrange
		var updateRequest =
			new UpdateExerciseTypeRequest(5, "Name", Guid.NewGuid().ToString(), "Description", "ImageUrl");
		var command = new UpdateExerciseTypeCommand(updateRequest);

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
	public async Task UpdateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		// - Seed data
		var exerciseType = _exerciseTypes.First();
		var updateRequest = new UpdateExerciseTypeRequest(exerciseType.Id, exerciseType.Name, Guid.NewGuid().ToString(),
			exerciseType.Description, exerciseType.ImageUrl);
		var command = new UpdateExerciseTypeCommand(updateRequest);


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
	public async Task
		UpdateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenExerciseTypeWithSameNameAlreadyExists()
	{
		// Arrange
		var storedExerciseType = await DbContext.ExerciseTypes.FindAsync(_exerciseTypes[2].Id);
		var updateRequest = new UpdateExerciseTypeRequest(_exerciseTypes[2].Id, _exerciseTypes[1].Name,
			storedExerciseType?.Concurrency, _exerciseTypes[2].Description, _exerciseTypes[2].ImageUrl);
		var command = new UpdateExerciseTypeCommand(updateRequest);

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
