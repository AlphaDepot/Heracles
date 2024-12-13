using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Features.Equipments.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class RemoveEquipmentCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Equipments.AddRange(_equipments);
		DbContext.SaveChanges();
		_handler = new RemoveEquipmentCommandHandler(DbContext);
	}

	private readonly List<Equipment> _equipments = EquipmentData.Equipments();
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
