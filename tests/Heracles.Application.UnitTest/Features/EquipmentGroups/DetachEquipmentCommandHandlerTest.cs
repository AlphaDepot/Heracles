using FluentResults;
using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.EquipmentGroups;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class DetachEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public new void Setup()
	{
		_equipmentGroups = EquipmentGroupRepository
			.Query()
			.Include(x => x.Equipments)
			.ToList();
		_handler = new DetachEquipmentCommandHandler(EquipmentGroupRepository,
			EquipmentRepository
		);
	}

	private List<EquipmentGroup> _equipmentGroups;
	private DetachEquipmentCommandHandler _handler;


	[Test]
	public async Task DetachEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange

		var detachRequest = new DetachEquipmentGroupRequest(_equipmentGroups.First().Id,
			_equipmentGroups.First().Equipments!.First().Id);
		var command = new DetachEquipmentGroupCommand(detachRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task DetachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var detachRequest = new DetachEquipmentGroupRequest(1000, _equipmentGroups.First().Equipments!.First().Id);
		var command = new DetachEquipmentGroupCommand(detachRequest);

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
	public async Task DetachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var detachRequest = new DetachEquipmentGroupRequest(_equipmentGroups.First().Id, 1000);
		var command = new DetachEquipmentGroupCommand(detachRequest);
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
	public async Task DetachEquipmentCommandHandler_ShouldReturnInvalidRequestErrorResult_WhenEquipmentNotAttached()
	{
		var group = _equipmentGroups.FirstOrDefault();
		var otherGroup = _equipmentGroups.Skip(1).FirstOrDefault();

		var equipment = otherGroup?.Equipments?.FirstOrDefault();

		Assert.That(group, Is.Not.Null);
		Assert.That(otherGroup, Is.Not.Null);
		Assert.That(equipment, Is.Not.Null);

		var detachRequest = new DetachEquipmentGroupRequest(group!.Id, equipment!.Id);
		var command = new DetachEquipmentGroupCommand(detachRequest);

		var result = await _handler.Handle(command, CancellationToken.None);

		Assert.That(result, Is.Not.Null);
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
	}
}
