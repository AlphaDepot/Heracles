using FluentResults;
using Heracles.Application.Features.EquipmentGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "EquipmentGroups")]
public class GetPagedEquipmentGroupsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_equipmentGroups = EquipmentGroupRepository.Query().ToList();

		_handler = new GetPagedEquipmentGroupsQueryHandler(EquipmentGroupRepository);
	}

	private List<EquipmentGroup> _equipmentGroups;
	private GetPagedEquipmentGroupsQueryHandler _handler;

	[Test]
	public async Task GetEquipmentGroupsQueryHandler_ShouldReturnPagedEquipmentGroups()
	{
		// Arrange
		var request = new QueryRequest { PageNumber = 1, PageSize = 10 };
		var query = new GetPagedEquipmentGroupsQuery(request);


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<EquipmentGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(3));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(_equipmentGroups.First().Name));
		Assert.That(result.Value.PageNumber, Is.EqualTo(1));
		Assert.That(result.Value.PageSize, Is.EqualTo(10));
		Assert.That(result.Value.TotalPages, Is.EqualTo(1));
		Assert.That(result.Value.TotalItems, Is.EqualTo(3));
	}

	[Test]
	public async Task GetEquipmentGroupsQueryHandler_ShouldReturnEmptyPagedEquipmentGroups()
	{
		// Arrange
		var allGroups = EquipmentGroupRepository.QueryTracking().ToList();
		var allEquipments = EquipmentRepository.QueryTracking().ToList();

		foreach (var group in allGroups)
		{
			await EquipmentGroupRepository.RemoveAsync(group);
		}

		foreach (var equipment in allEquipments)
		{
			await EquipmentRepository.RemoveAsync(equipment);
		}

		await EquipmentGroupRepository.SaveChangesAsync();
		await EquipmentRepository.SaveChangesAsync();

		var query = new GetPagedEquipmentGroupsQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<EquipmentGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task GetEquipmentGroupsQueryHandler_ShouldReturnPagedEquipmentGroupsWithSearchTerm()
	{
		// Arrange
		var query = new GetPagedEquipmentGroupsQuery(new QueryRequest { SearchTerm = _equipmentGroups.First().Name });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<EquipmentGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(_equipmentGroups.First().Name));
	}
}
