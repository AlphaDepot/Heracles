using FluentResults;
using Heracles.Application.Features.MuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.MuscleGroups;

namespace Heracles.Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class UpdateMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleGroups = MuscleGroupsRepository.Query().ToList();
		_handler = new UpdateMuscleGroupCommandHandler(MuscleGroupsRepository);
	}

	private List<MuscleGroup> _muscleGroups;
	private UpdateMuscleGroupCommandHandler _handler;


	[Test]
	public async Task UpdateMuscleGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var storedMuscleGroup = await MuscleGroupsRepository.GetByIdAsync(muscleGroup.Id);
		var updateRequest =
			new UpdateMuscleGroupRequest(muscleGroup.Id, muscleGroup.Name, storedMuscleGroup?.Concurrency);
		var command = new UpdateMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedMuscleGroup = await MuscleGroupsRepository.GetByIdAsync(muscleGroup.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedMuscleGroup, Is.Not.Null);
		Assert.That(updatedMuscleGroup.Id, Is.EqualTo(muscleGroup.Id));
		Assert.That(updatedMuscleGroup.Concurrency, Is.Not.Null);
		Assert.That(updatedMuscleGroup.Name, Is.EqualTo(muscleGroup.Name));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedMuscleGroup.CreatedAt, Is.EqualTo(muscleGroup.CreatedAt).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedMuscleGroup.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}

	[Test]
	public async Task UpdateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var updateRequest = new UpdateMuscleGroupRequest(1, "Name", Guid.NewGuid().ToString());
		var command = new UpdateMuscleGroupCommand(updateRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task UpdateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenMuscleGroupNotFound()
	{
		// Arrange
		var updateRequest = new UpdateMuscleGroupRequest(1000, "Name", Guid.NewGuid().ToString());
		var command = new UpdateMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task UpdateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var updateRequest = new UpdateMuscleGroupRequest(muscleGroup.Id, muscleGroup.Name, Guid.NewGuid().ToString());
		var command = new UpdateMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.ConcurrencyError));
	}

	[Test]
	public async Task UpdateMuscleGroupCommandHandler_ShouldReturnErrorResult_WhenNamingConflict()
	{
		// Arrange
		var storedMuscleGroup = await MuscleGroupsRepository.GetByIdAsync(_muscleGroups[2].Id);
		var updateRequest =
			new UpdateMuscleGroupRequest(_muscleGroups[2].Id, _muscleGroups[1].Name, storedMuscleGroup?.Concurrency);
		var command = new UpdateMuscleGroupCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
	}
}
