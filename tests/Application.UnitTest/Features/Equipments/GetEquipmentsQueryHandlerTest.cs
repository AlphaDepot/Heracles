using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Features.Equipments.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class GetPagedEquipmentsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.Equipments.AddRange(_equipments.Take(4));
		DbContext.SaveChanges();

		_handler = new GetPagedEquipmentsQueryHandler(DbContext);
	}

	private readonly List<Equipment> _equipments = EquipmentData.Equipments();
	private GetPagedEquipmentsQueryHandler _handler;


	[Test]
	public async Task GetEquipmentsQueryHandler_ShouldReturnPagedEquipments()
	{
		// Arrange
		var query = new GetPagedEquipmentsQuery(new QueryRequest());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<Equipment>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(4));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Type, Is.EqualTo(_equipments.First().Type));
	}

	[Test]
	public async Task GetEquipmentsQueryHandler_ShouldReturnEmptyPagedEquipments()
	{
		// Arrange
		// - Clear all equipments for this test only
		DbContext.Equipments.RemoveRange(DbContext.Equipments);
		await DbContext.SaveChangesAsync();
		var query = new GetPagedEquipmentsQuery(new QueryRequest());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<Equipment>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task GetEquipmentsQueryHandler_ShouldReturnPagedEquipmentsWithSearchTerm()
	{
		// Arrange
		var query = new GetPagedEquipmentsQuery(new QueryRequest { SearchTerm = _equipments.First().Type });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<Equipment>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Type, Is.EqualTo(_equipments.First().Type));
	}

	[Test]
	public async Task GetEquipmentsQueryHandler_ShouldReturnPagedEquipmentsWithSortOrder()
	{
		// Arrange
		var sortedEquipments = _equipments.OrderByDescending(x => x.Type).ToList();
		var query = new GetPagedEquipmentsQuery(new QueryRequest
		{
			SortBy = "Type",
			SortOrder = "desc"
		});

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<Equipment>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(4));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(sortedEquipments.First().Id));
		Assert.That(result.Value.Data.First().Type, Is.EqualTo(sortedEquipments.First().Type));
	}
}
