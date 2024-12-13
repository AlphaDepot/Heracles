using NUnit.Framework;
using System.Net.Http.Json;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Features.Equipments.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the EquipmentTypesController.
/// </summary>
public class TestEquipmentsController : BaseIntegrationTest
{
    private const string BaseUrl = "/api/Equipments";

    [SetUp]
    public void SetUp()
    {
        // Initialization code if needed
    }

    /// <summary>
    /// Test to ensure that the GetEquipments endpoint returns a list of equipments.
    /// </summary>
    [Test]
    public async Task GetEquipments_ReturnsEquipments()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<Equipment>>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data.Count, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the GetEquipmentById endpoint returns the correct equipment.
    /// </summary>
    [Test]
    public async Task GetEquipmentById_ReturnsEquipment()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);
        var result = await response.Content.ReadFromJsonAsync<Equipment>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
    }

    /// <summary>
    /// Test to ensure that the CreateEquipment endpoint correctly creates an equipment and returns its Id.
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
        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the UpdateEquipment endpoint correctly updates an equipment.
    /// </summary>
    [Test]
    public async Task UpdateEquipment_ReturnsUpdatedEquipment()
    {
        // Arrange
        var random = new Random();
        var existingEquipment = await Client.GetFromJsonAsync<Equipment>($"{BaseUrl}/2");
		var equipment = new UpdateEquipmentRequest(2, "Barbell" + random.Next(1, 10), existingEquipment?.Concurrency, 100, 100);
        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{equipment.Id}", equipment);
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteEquipment endpoint correctly deletes an equipment.
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
