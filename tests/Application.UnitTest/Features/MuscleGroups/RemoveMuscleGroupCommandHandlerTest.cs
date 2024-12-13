using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class RemoveMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleGroups.AddRange(_muscleGroups);
		DbContext.SaveChangesAsync();

		_handler = new RemoveMuscleGroupCommandHandler(DbContext);
	}

	private readonly List<MuscleGroup> _muscleGroups = ExerciseTypeData.MuscleGroups().Take(3).ToList();
	private RemoveMuscleGroupCommandHandler _handler;

	[Test]
	public async Task RemoveMuscleGroupCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveMuscleGroupCommand(1);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenMuscleGroupNotFound()
	{
		// Arrange
		var command = new RemoveMuscleGroupCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveMuscleGroupCommand(100, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
