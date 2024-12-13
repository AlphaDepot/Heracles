using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class UpdateEquipmentGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data

		DbContext.EquipmentGroups.AddRange(_equipmentGroups);
		DbContext.SaveChanges();

		_handler = new UpdateEquipmentGroupCommandHandler(DbContext);

		_updateRequest = new UpdateEquipmentGroupRequest(_equipmentGroups[0].Id, _equipmentGroups[0].Name,
			_equipmentGroups[0].Concurrency);
	}

	private readonly List<EquipmentGroup> _equipmentGroups = EquipmentData.EquipmentGroups();
	private UpdateEquipmentGroupCommandHandler _handler;
	private UpdateEquipmentGroupRequest _updateRequest;


	[Test]
	public async Task UpdateEquipmentGroupCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var equipmentGroup = EquipmentData.EquipmentGroups().First();
		// - Delay 1 milliseconds to ensure the created at and updated at are different
		await Task.Delay(1);

		var command = new UpdateEquipmentGroupCommand(_updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedEquipmentGroup = await DbContext.EquipmentGroups.FindAsync(_equipmentGroups.First().Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedEquipmentGroup, Is.Not.Null);
		Assert.That(updatedEquipmentGroup.Id, Is.EqualTo(equipmentGroup.Id));
		Assert.That(updatedEquipmentGroup.Concurrency, Is.Not.Null);
		Assert.That(updatedEquipmentGroup.Name, Is.EqualTo(equipmentGroup.Name));

		// Assuming a leeway of 5 seconds
		Assert.That(updatedEquipmentGroup.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
		Assert.That(updatedEquipmentGroup.UpdatedAt, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(5)));
	}

	[Test]
	public async Task UpdateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new UpdateEquipmentGroupCommand(_updateRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}

	[Test]
	public async Task UpdateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var updateRequest = new UpdateEquipmentGroupRequest(5, _updateRequest.Name, _updateRequest.Concurrency);
		var command = new UpdateEquipmentGroupCommand(updateRequest);

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
	public async Task UpdateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenConcurrencyError()
	{
		// Arrange
		var updateRequest =
			new UpdateEquipmentGroupRequest(_updateRequest.Id, _updateRequest.Name, Guid.NewGuid().ToString());
		var command = new UpdateEquipmentGroupCommand(updateRequest);

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
	public async Task UpdateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenNamingConflict()
	{
		// Arrange

		var storedEquipmentGroup = await DbContext.EquipmentGroups.FindAsync(_equipmentGroups[2].Id);
		var updateRequest = new UpdateEquipmentGroupRequest(_equipmentGroups[2].Id, _equipmentGroups[1].Name,
			storedEquipmentGroup?.Concurrency);
		var command = new UpdateEquipmentGroupCommand(updateRequest);


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
