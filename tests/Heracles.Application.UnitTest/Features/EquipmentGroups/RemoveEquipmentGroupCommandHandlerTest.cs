using FluentResults;
using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class RemoveEquipmentGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipmentGroups = EquipmentGroupRepository
			.Query()
			.ToList();

		_handler = new RemoveEquipmentGroupCommandHandler(EquipmentGroupRepository);
	}

	private RemoveEquipmentGroupCommandHandler _handler;
	private List<EquipmentGroup> _equipmentGroups;


	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var equipmentGroup = _equipmentGroups.First();
		var command = new RemoveEquipmentGroupCommand(equipmentGroup.Id);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var command = new RemoveEquipmentGroupCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveEquipmentGroupCommand(100, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
