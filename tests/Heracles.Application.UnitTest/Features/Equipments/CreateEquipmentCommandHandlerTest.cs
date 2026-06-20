using FluentResults;
using Heracles.Application.Features.Equipments.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.Equipments;

namespace Heracles.Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class CreateEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipments = EquipmentRepository.Query().ToList();
		_handler = new CreateEquipmentCommandHandler(EquipmentRepository);
	}

	private List<Equipment> _equipments;
	private CreateEquipmentCommandHandler _handler;

	[Test]
	public async Task CreateEquipmentCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateEquipmentRequest("Unique Equipment Type Name", 1, 1);
		var command = new CreateEquipmentCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var equipment = EquipmentRepository.Query()
			.FirstOrDefault(x => x.Id == result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(equipment, Is.Not.Null);
		Assert.That(equipment.Id, Is.EqualTo(result.Value));
		Assert.That(equipment.CreatedAt, Is.TypeOf<DateTime>());
		Assert.That(equipment.UpdatedAt, Is.TypeOf<DateTime>());
		Assert.That(equipment.Concurrency, Is.Not.Null);
		Assert.That(equipment.Type, Is.EqualTo(createRequest.Type));
	}

	[Test]
	public async Task CreateEquipmentCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		var createRequest = new CreateEquipmentRequest(_equipments.First().Type, _equipments.First().Weight,
			_equipments.First().Resistance);
		var command = new CreateEquipmentCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
	}

	[Test]
	public async Task CreateEquipmentCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new CreateEquipmentCommand(
			new CreateEquipmentRequest(_equipments.First().Type, _equipments.First().Weight,
				_equipments.First().Resistance), false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
