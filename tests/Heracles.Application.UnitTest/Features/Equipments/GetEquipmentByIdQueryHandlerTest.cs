using FluentResults;
using Heracles.Application.Features.Equipments.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class GetEquipmentByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipments = EquipmentRepository.Query().ToList();
		_handler = new GetEquipmentByIdQueryHandler(EquipmentRepository);
	}

	private List<Equipment> _equipments;
	private GetEquipmentByIdQueryHandler _handler;


	[Test]
	public async Task GetEquipmentByIdQueryHandler_ShouldReturnEquipment()
	{
		// Arrange
		var query = new GetEquipmentByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<Equipment>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Type, Is.EqualTo(_equipments.First().Type));
	}


	[Test]
	public async Task GetEquipmentByIdQueryHandler_ShouldReturnErrorResult_WhenEquipmentNotFound()
	{
		// Arrange
		var query = new GetEquipmentByIdQuery(1000);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<Equipment>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
