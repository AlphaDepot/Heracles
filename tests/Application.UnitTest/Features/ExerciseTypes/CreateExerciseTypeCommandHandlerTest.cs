using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class CreateExerciseTypeCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseTypes.AddRange(_exerciseTypes);
		DbContext.SaveChangesAsync();
		_handler = new CreateExerciseTypeCommandHandler(DbContext);
	}

	private readonly List<ExerciseType> _exerciseTypes = ExerciseTypeData.ExerciseTypes();
	private CreateExerciseTypeCommandHandler _handler;

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateExerciseTypeRequest("Unique Exercise Type Name", "Exercise Type Description",
			"Exercise Type Image Url");
		var command = new CreateExerciseTypeCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var exerciseType = await DbContext.ExerciseTypes.FindAsync(result.Value);

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
		Assert.That(exerciseType.ImageUrl, Is.EqualTo(createRequest.ImageUrl));
	}

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		// - Seed data
		var exerciseType = _exerciseTypes.First();

		// - Create command
		var createRequest =
			new CreateExerciseTypeRequest(exerciseType.Name, exerciseType.Description, exerciseType.ImageUrl);
		var command = new CreateExerciseTypeCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}

	[Test]
	public async Task CreateExerciseTypeCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();

		var command = new CreateExerciseTypeCommand(
			new CreateExerciseTypeRequest(exerciseType.Name, exerciseType.Description, exerciseType.ImageUrl), false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
