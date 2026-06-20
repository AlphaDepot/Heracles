using FluentResults;
using Heracles.Application.Features.EquipmentGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class GetEquipmentGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[Test]
	public async Task GetEquipmentGroupByIdQueryHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		// - Seed data
		var equipmentGroups = EquipmentGroupRepository.Query().ToList();
		var equipmentGroup = equipmentGroups.First();

		// - Create query
		var query = new GetEquipmentGroupByIdQuery(equipmentGroup.Id);
		var handler = new GetEquipmentGroupByIdQueryHandler(EquipmentGroupRepository);

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
		var handler = new GetEquipmentGroupByIdQueryHandler(EquipmentGroupRepository);

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<EquipmentGroup>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
