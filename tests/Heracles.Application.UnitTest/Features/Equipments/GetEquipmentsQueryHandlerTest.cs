using FluentResults;
using Heracles.Application.Features.Equipments.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.Equipments;

[TestFixture(Category = "Equipments")]
public class GetPagedEquipmentsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipments = EquipmentRepository
			.Query()
			.ToList();

		_handler = new GetPagedEquipmentsQueryHandler(EquipmentRepository);
	}

	private List<Equipment> _equipments;
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
		var allEquipments = EquipmentRepository.QueryTracking().ToList();

		foreach (var eq in allEquipments)
		{
			await EquipmentRepository.RemoveAsync(eq);
		}

		await EquipmentRepository.SaveChangesAsync();

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
