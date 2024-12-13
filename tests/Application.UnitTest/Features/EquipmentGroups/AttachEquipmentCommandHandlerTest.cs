using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class AttachEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.EquipmentGroups.AddRange(_equipmentGroups);
		DbContext.SaveChanges();

		_handler = new AttachEquipmentCommandHandler(DbContext);
	}

	private readonly List<EquipmentGroup> _equipmentGroups = EquipmentData.EquipmentGroups();
	private AttachEquipmentCommandHandler _handler;

	[Test]
	public async Task AttachEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var equipment = EquipmentData.Equipments().Last();
		var equipmentRequest = new AttachEquipmentRequest(_equipmentGroups.First().Id, equipment.Id);
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
		var equipment = EquipmentData.Equipments().First();
		var equipmentRequest = new AttachEquipmentRequest(1000, equipment.Id);
		var addRequest = new AttachEquipmentCommand(equipmentRequest);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task AttachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var equipmentRequest = new AttachEquipmentRequest(_equipmentGroups.First().Id, 1000);
		var addRequest = new AttachEquipmentCommand(equipmentRequest);

		// Act
		var result = await _handler.Handle(addRequest, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}
}
