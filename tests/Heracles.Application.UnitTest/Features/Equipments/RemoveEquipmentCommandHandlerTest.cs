using FluentResults;
using Heracles.Application.Features.Equipments.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class RemoveEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipments = EquipmentRepository.Query().ToList();
		_handler = new RemoveEquipmentCommandHandler(EquipmentRepository);
	}

	private List<Equipment> _equipments;
	private RemoveEquipmentCommandHandler _handler;


	[Test]
	public async Task RemoveEquipmentCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveEquipmentCommand(1);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveEquipmentCommandHandler_ShouldReturnErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		// - Create command
		var command = new RemoveEquipmentCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveEquipmentCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveEquipmentCommand(100, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
