using FluentResults;
using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.EquipmentGroups;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class AttachEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipmentGroups = EquipmentGroupRepository.Query().ToList();
		_equipments = EquipmentRepository.Query().ToList();

		_handler = new AttachEquipmentCommandHandler(
			EquipmentGroupRepository,
			EquipmentRepository
		);
	}

	private List<EquipmentGroup> _equipmentGroups;
	private List<Equipment> _equipments;
	private AttachEquipmentCommandHandler _handler;

	[Test]
	public async Task AttachEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var equipment = _equipments.Last();
		var equipmentRequest = new AttachEquipmentGroupRequest(_equipmentGroups.First().Id, equipment.Id);
		var addRequest = new AttachEquipmentCommand(equipmentRequest);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task AttachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var equipment = _equipments.First();
		var equipmentRequest = new AttachEquipmentGroupRequest(1000, equipment.Id);
		var addRequest = new AttachEquipmentCommand(equipmentRequest);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);

		var error = result.Errors.First();

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task AttachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var equipmentRequest = new AttachEquipmentGroupRequest(_equipmentGroups.First().Id, 1000);
		var addRequest = new AttachEquipmentCommand(equipmentRequest);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailed, Is.True);

		var error = result.Errors.First();


		//Console.WriteLine(JsonSerializer.Serialize(result));
		Assert.That(error, Is.Not.Null);
		Assert.That(error.Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
