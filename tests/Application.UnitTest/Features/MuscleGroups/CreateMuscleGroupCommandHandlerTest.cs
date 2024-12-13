using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class CreateMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleGroups.AddRange(_muscleGroups);
		DbContext.SaveChangesAsync();

		_handler = new CreateMuscleGroupCommandHandler(DbContext);
	}

	private readonly List<MuscleGroup> _muscleGroups = ExerciseTypeData.MuscleGroups().Take(3).ToList();
	private CreateMuscleGroupCommandHandler _handler;


	[Test]
	public async Task CreateMuscleGroupCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateMuscleGroupRequest("Unique Muscle Group Name");
		var command = new CreateMuscleGroupCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var muscleGroup = await DbContext.MuscleGroups.FindAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(muscleGroup, Is.Not.Null);
		Assert.That(muscleGroup.Id, Is.EqualTo(result.Value));
		Assert.That(muscleGroup.CreatedAt, Is.TypeOf<DateTime>());
		Assert.That(muscleGroup.UpdatedAt, Is.TypeOf<DateTime>());
		Assert.That(muscleGroup.Concurrency, Is.Not.Null);
		Assert.That(muscleGroup.Name, Is.EqualTo(request.Name));
	}

	[Test]
	public async Task CreateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		var request = new CreateMuscleGroupRequest(_muscleGroups.First().Name);
		var command = new CreateMuscleGroupCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}

	[Test]
	public async Task CreateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var request = new CreateMuscleGroupRequest(_muscleGroups.First().Name);
		var command = new CreateMuscleGroupCommand(request, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
