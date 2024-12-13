using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Features.Equipments.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class UpdateEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Equipments.AddRange(_equipments);
		DbContext.SaveChanges();
		_handler = new UpdateEquipmentCommandHandler(DbContext);
	}

	private readonly List<Equipment> _equipments = EquipmentData.Equipments();
	private UpdateEquipmentCommandHandler _handler;

	[Test]
	public async Task UpdateEquipmentCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// - Delay 1 milliseconds to ensure the created at and updated at are different
		await Task.Delay(1);
		var storedEquipment = await DbContext.Equipments.FindAsync(_equipments.First().Id);
		var updateRequest = new UpdateEquipmentRequest(_equipments.First().Id, _equipments.First().Type,
			storedEquipment?.Concurrency, _equipments.First().Weight, _equipments.First().Resistance);
		var command = new UpdateEquipmentCommand(updateRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedEquipment = await DbContext.Equipments.FindAsync(_equipments.First().Id);

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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.ConcurrencyError));
	}

	[Test]
	public async Task UpdateEquipmentCommandHandler_ShouldReturnErrorResult_WhenTypeIsDuplicated()
	{
		// Arrange
		var storedEquipment = await DbContext.Equipments.FindAsync(_equipments[2].Id);
		var updateRequest = new UpdateEquipmentRequest(_equipments[2].Id, _equipments[1].Type,
			storedEquipment?.Concurrency,
			_equipments[2].Weight, _equipments[2].Resistance);
		var command = new UpdateEquipmentCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}
}
