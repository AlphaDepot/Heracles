using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class GetEquipmentGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[Test]
	public async Task GetEquipmentGroupByIdQueryHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// - Seed data
		var equipmentGroups = EquipmentData.EquipmentGroups();
		var equipmentGroup = equipmentGroups.First();
		DbContext.EquipmentGroups.AddRange(equipmentGroups);
		await DbContext.SaveChangesAsync();
		// - Create query
		var query = new GetEquipmentGroupByIdQuery(equipmentGroup.Id);
		var handler = new GetEquipmentGroupByIdQueryHandler(DbContext);

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<EquipmentGroup>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value.Id, Is.EqualTo(equipmentGroup.Id));
	}

	[Test]
	public async Task GetEquipmentGroupByIdQueryHandler_ShouldReturnNotFoundErrorResult_WhenEquipmentGroupNotFound()
	{
		// Arrange
		var query = new GetEquipmentGroupByIdQuery(1000);
		var handler = new GetEquipmentGroupByIdQueryHandler(DbContext);

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<EquipmentGroup>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error.Type, Is.EqualTo(ErrorCodes.NotFound));
	}
}
