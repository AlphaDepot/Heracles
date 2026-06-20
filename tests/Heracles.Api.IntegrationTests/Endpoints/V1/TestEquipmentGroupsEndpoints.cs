using System.Net.Http.Json;
using System.Text.Json;
using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.EquipmentGroups;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the EquipmentGroupsController.
/// </summary>
public class TestEquipmentGroupsEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.EquipmentGroups;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededEquipment()
	{
		using var context = GetDbContext();

		var count = context.EquipmentGroups.Count();

		Console.WriteLine("Equipment Groups count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetEquipmentGroups endpoint returns a list of equipment groups.
	/// </summary>
	[Test]
	public async Task GetEquipmentGroups_ReturnsEquipmentGroups()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<EquipmentGroup>>>();
		Console.WriteLine(JsonSerializer.Serialize(result));
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data.Count, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the GetEquipmentGroupById endpoint returns the correct equipment group.
	/// </summary>
	[Test]
	public async Task GetEquipmentGroupById_ReturnsEquipmentGroup()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<EquipmentGroup>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}

	/// <summary>
	///     Test to ensure that the CreateEquipmentGroup endpoint correctly creates a new equipment group.
	/// </summary>
	[Test]
	public async Task CreateEquipmentGroup_ReturnsCreatedEquipmentGroup()
	{
		// Arrange
		var equipmentGroup = new CreateEquipmentGroupRequest("Test Equipment Group");

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, equipmentGroup);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result?.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateEquipmentGroup endpoint correctly updates an existing equipment group.
	/// </summary>
	[Test]
	public async Task UpdateEquipmentGroup_ReturnsUpdatedEquipmentGroup()
	{
		// Arrange
		var res = await Client.GetFromJsonAsync<ApiResponse<EquipmentGroup>>($"{BaseUrl}/2");
		var equipmentGroup = new UpdateEquipmentGroupRequest(2, "Test Equipment Group Updated 2",
			res?.Data.Concurrency);
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", equipmentGroup);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the DeleteEquipmentGroup endpoint correctly deletes an existing equipment group.
	/// </summary>
	[Test]
	public async Task DeleteEquipmentGroup_ReturnsDeletedEquipmentGroup()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the AddEquipmentToGroup endpoint correctly adds an equipment to an existing group.
	/// </summary>
	[Test]
	public async Task AddEquipmentToGroup_ReturnsEquipmentGroup()
	{
		// Arrange
		var equipmentGroup = new AttachEquipmentGroupRequest(2, 1);


		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/2/add", equipmentGroup);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the RemoveEquipmentFromGroup endpoint correctly removes an equipment from an existing group.
	/// </summary>
	[Test]
	public async Task RemoveEquipmentFromGroup_ReturnsEquipmentGroup()
	{
		await using var context = GetDbContext();

		// Find ANY group that actually has equipment attached
		var group = context.EquipmentGroups
			.Include(x => x.Equipments)
			.FirstOrDefault(g => g.Equipments.Any());


		Assert.That(group, Is.Not.Null, "No equipment group has any equipment attached.");
		Assert.That(group.Equipments.Any(), Is.True, "Selected group has no equipment.");

		var equipment = group.Equipments.First();

		var request = new DetachEquipmentGroupRequest(group.Id, equipment.Id);

		// Act — use the REAL group ID
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/{group.Id}/remove", request);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
