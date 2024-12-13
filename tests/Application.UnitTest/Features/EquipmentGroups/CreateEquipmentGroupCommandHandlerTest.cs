using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class CreateEquipmentGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.EquipmentGroups.AddRange(_equipmentGroups);
		DbContext.SaveChanges();

		_handler = new CreateEquipmentGroupCommandHandler(DbContext);
	}

	private readonly List<EquipmentGroup> _equipmentGroups = EquipmentData.EquipmentGroups();
	private CreateEquipmentGroupCommandHandler _handler;


	[Test]
	public async Task CreateEquipmentGroupCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateEquipmentGroupRequest("Unique Equipment Group Name");
		var command = new CreateEquipmentGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var equipmentGroup = await DbContext.EquipmentGroups.FindAsync(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
		Assert.That(equipmentGroup, Is.Not.Null);
		Assert.That(equipmentGroup.Id, Is.EqualTo(result.Value));
		Assert.That(equipmentGroup.CreatedAt, Is.TypeOf<DateTime>());
		Assert.That(equipmentGroup.UpdatedAt, Is.TypeOf<DateTime>());
		Assert.That(equipmentGroup.Concurrency, Is.Not.Null);
		Assert.That(equipmentGroup.Name, Is.EqualTo(createRequest.Name));
	}

	[Test]
	public async Task CreateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenNameIsDuplicated()
	{
		// Arrange
		var createRequest = new CreateEquipmentGroupRequest(_equipmentGroups.First().Name);
		var command = new CreateEquipmentGroupCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
	}

	[Test]
	public async Task CreateEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new CreateEquipmentGroupCommand(
			new CreateEquipmentGroupRequest(_equipmentGroups.First().Name), false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
