using FluentResults;
using Heracles.Application.Features.MuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.MuscleGroups;

namespace Heracles.Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class CreateMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleGroups = MuscleGroupsRepository.Query().ToList();
		_handler = new CreateMuscleGroupCommandHandler(MuscleGroupsRepository);
	}

	private List<MuscleGroup> _muscleGroups;
	private CreateMuscleGroupCommandHandler _handler;


	[Test]
	public async Task CreateMuscleGroupCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var request = new CreateMuscleGroupRequest("Unique Muscle Group Name");
		var command = new CreateMuscleGroupCommand(request);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var muscleGroup = await MuscleGroupsRepository.GetByIdAsync(result.Value);
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
