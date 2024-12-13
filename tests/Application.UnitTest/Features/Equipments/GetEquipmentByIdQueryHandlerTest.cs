using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Features.Equipments.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class GetEquipmentByIdQueryHandlerTest : HandlerBaseUnitTest
{
	private readonly List<Equipment> _equipments = EquipmentData.Equipments();
	private GetEquipmentByIdQueryHandler _handler;

	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Equipments.AddRange(_equipments);
		DbContext.SaveChanges();
		_handler = new GetEquipmentByIdQueryHandler(DbContext);
	}




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
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}
}
