using FluentResults;
using Heracles.Application.Features.MuscleGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class RemoveMuscleGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleGroups = MuscleGroupsRepository.Query().ToList();
		_handler = new RemoveMuscleGroupCommandHandler(MuscleGroupsRepository);
	}

	private List<MuscleGroup> _muscleGroups;
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
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
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
