using System.Net.Http.Json;
using System.Text.Json;
using Application.Common.Responses;
using Application.Features.EquipmentGroups;
using Application.Features.EquipmentGroups.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
///     This class contains integration tests for the EquipmentGroupsController.
/// </summary>
public class TestEquipmentGroupsController : BaseIntegrationTest
{
	private const string BaseUrl = "/api/EquipmentGroups";

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
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

		var result = await response.Content.ReadFromJsonAsync<PagedResponse<EquipmentGroup>>();
		Console.WriteLine(JsonSerializer.Serialize(result));
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Count, Is.GreaterThan(0));
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

		var result = await response.Content.ReadFromJsonAsync<EquipmentGroup>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(3));
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

		var result = await response.Content.ReadFromJsonAsync<int>();

		// Assert
		Assert.That(result, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateEquipmentGroup endpoint correctly updates an existing equipment group.
	/// </summary>
	[Test]
	public async Task UpdateEquipmentGroup_ReturnsUpdatedEquipmentGroup()
	{
		// Arrange
		var existingEquipmentGroup = await Client.GetFromJsonAsync<EquipmentGroup>($"{BaseUrl}/2");
		var equipmentGroup = new UpdateEquipmentGroupRequest(2, "Test Equipment Group Updated 2",
			existingEquipmentGroup?.Concurrency);
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
		var equipmentGroup = new AttachEquipmentRequest(2, 1);


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
		// Arrange
		var equipmentGroup = new DetachEquipmentRequest(2, 1);

		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/2/remove", equipmentGroup);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
