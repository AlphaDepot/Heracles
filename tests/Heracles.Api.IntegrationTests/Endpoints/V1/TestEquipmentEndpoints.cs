using System.Net.Http.Json;
using Heracles.Application.Features.Equipments.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.Equipments;
using Heracles.Shared.Responses;
using Newtonsoft.Json;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the EquipmentTypesController.
/// </summary>
public class TestEquipmentEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.Equipment;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededEquipment()
	{
		using var context = GetDbContext();

		var count = context.Equipments.Count();

		Console.WriteLine("Equipment count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetEquipments endpoint returns a list of equipments.
	/// </summary>
	[Test]
	public async Task GetEquipments_ReturnsEquipments()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);
		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<Equipment>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data.Count, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the GetEquipmentById endpoint returns the correct equipment.
	/// </summary>
	[Test]
	public async Task GetEquipmentById_ReturnsEquipment()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);
		var result = await response.Content.ReadFromJsonAsync<ApiResponse<Equipment>>();



		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}

	/// <summary>
	///     Test to ensure that the CreateEquipment endpoint correctly creates an equipment and returns its Id.
	/// </summary>
	[Test]
	public async Task CreateEquipment_ReturnsCreatedEquipment()
	{
		// Arrange
		var random = new Random();
		var equipment = new CreateEquipmentRequest("Barbell*" + random.Next(1, 1000), 45, 4);

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, equipment);
		HandleResponseFailure(response);
		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result?.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateEquipment endpoint correctly updates an equipment.
	/// </summary>
	[Test]
	public async Task UpdateEquipment_ReturnsUpdatedEquipment()
	{
		// Arrange
		var random = new Random();
		var iniRes = await Client.GetFromJsonAsync<ApiResponse<Equipment>>($"{BaseUrl}/2");
		var equipment =
			new UpdateEquipmentRequest(2, "Barbell" + random.Next(1, 10), iniRes?.Data?.Concurrency, 100, 100);
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/{equipment.Id}", equipment);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	///     Test to ensure that the DeleteEquipment endpoint correctly deletes an equipment.
	/// </summary>
	[Test]
	public async Task DeleteEquipment_ReturnsNoContent()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/1");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
