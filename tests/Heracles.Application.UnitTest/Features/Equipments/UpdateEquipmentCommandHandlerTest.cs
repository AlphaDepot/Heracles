using FluentResults;
using Heracles.Application.Features.Equipments.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.Equipments;

namespace Heracles.Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class UpdateEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipments = EquipmentRepository
			.Query()
			.ToList();

		_handler = new UpdateEquipmentCommandHandler(EquipmentRepository);
	}

	private List<Equipment> _equipments;
	private UpdateEquipmentCommandHandler _handler;

	[Test]
	public async Task UpdateEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// - Delay 1 milliseconds to ensure the created at and updated at are different
		await Task.Delay(1);
		var storedEquipment = EquipmentRepository.Query()
			.FirstOrDefault(x => x.Id == _equipments.First().Id);
		var updateRequest = new UpdateEquipmentRequest(_equipments.First().Id, _equipments.First().Type,
			storedEquipment?.Concurrency, _equipments.First().Weight, _equipments.First().Resistance);
		var command = new UpdateEquipmentCommand(updateRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedEquipment = EquipmentRepository.Query()
			.FirstOrDefault(x => x.Id == _equipments.First().Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedEquipment, Is.Not.Null);
		Assert.That(updatedEquipment.Id, Is.EqualTo(_equipments.First().Id));
		Assert.That(updatedEquipment.Concurrency, Is.Not.Null);
		Assert.That(updatedEquipment.Type, Is.EqualTo(_equipments.First().Type));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedEquipment.CreatedAt,
			Is.EqualTo(_equipments.First().CreatedAt).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedEquipment.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}

	[Test]
	public async Task UpdateEquipmentCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange

		var updateRequest = new UpdateEquipmentRequest(1, "Type", Guid.NewGuid().ToString(), 1, 1);
		var command = new UpdateEquipmentCommand(updateRequest, false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task UpdateEquipmentCommandHandler_ShouldReturnErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var updateRequest = new UpdateEquipmentRequest(5, "Type", Guid.NewGuid().ToString(), 1, 1);
		var command = new UpdateEquipmentCommand(updateRequest);


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
	public async Task UpdateEquipmentCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		var updateRequest = new UpdateEquipmentRequest(_equipments.First().Id, _equipments.First().Type,
			Guid.NewGuid().ToString(), _equipments.First().Weight, _equipments.First().Resistance);
		var command = new UpdateEquipmentCommand(updateRequest);

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
	public async Task UpdateEquipmentCommandHandler_ShouldReturnErrorResult_WhenTypeIsDuplicated()
	{
		// Arrange
		var storedEquipment = EquipmentRepository.Query()
			.FirstOrDefault(x => x.Id == _equipments[2].Id);
		var updateRequest = new UpdateEquipmentRequest(_equipments[2].Id, _equipments[1].Type,
			storedEquipment?.Concurrency,
			_equipments[2].Weight, _equipments[2].Resistance);
		var command = new UpdateEquipmentCommand(updateRequest);

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
