using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class RemoveEquipmentGroupCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.EquipmentGroups.AddRange(EquipmentData.EquipmentGroups());
		DbContext.SaveChanges();

		_handler = new RemoveEquipmentGroupCommandHandler(DbContext);
	}

	private RemoveEquipmentGroupCommandHandler _handler;

	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveEquipmentGroupCommand(1);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var command = new RemoveEquipmentGroupCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task RemoveEquipmentGroupCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveEquipmentGroupCommand(100, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
