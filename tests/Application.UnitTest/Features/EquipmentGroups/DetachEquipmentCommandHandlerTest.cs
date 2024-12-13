using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class DetachEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public new void Setup()
	{
		// Seed data

		DbContext.EquipmentGroups.AddRange(_equipmentGroups);
		DbContext.SaveChanges();

		_handler = new DetachEquipmentCommandHandler(DbContext);
	}

	private readonly List<EquipmentGroup> _equipmentGroups = EquipmentData.EquipmentGroups();

	private DetachEquipmentCommandHandler _handler;

	[Test]
	public async Task DetachEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange

		var detachRequest = new DetachEquipmentRequest(_equipmentGroups.First().Id,
			_equipmentGroups.First().Equipments!.First().Id);
		var command = new DetachEquipmentCommand(detachRequest);

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
		var detachRequest = new DetachEquipmentRequest(1000, _equipmentGroups.First().Equipments!.First().Id);
		var command = new DetachEquipmentCommand(detachRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task DetachEquipmentCommandHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var detachRequest = new DetachEquipmentRequest(_equipmentGroups.First().Id, 1000);
		var command = new DetachEquipmentCommand(detachRequest);
		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task DetachEquipmentCommandHandler_ShouldReturnInvalidRequestErrorResult_WhenEquipmentNotAttached()
	{
		// Arrange
		var detachRequest =
			new DetachEquipmentRequest(_equipmentGroups.First().Id, _equipmentGroups[2].Equipments!.First().Id);
		var command = new DetachEquipmentCommand(detachRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
	}
}
