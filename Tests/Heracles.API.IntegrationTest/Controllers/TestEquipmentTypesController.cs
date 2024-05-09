using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Equipments.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;
/// <summary>
/// This class contains integration tests for the EquipmentTypesController.
/// </summary>
public class TestEquipmentTypesController : BaseIntegrationTest
{
    const string BaseUrl = "/api/EquipmentTypes";
    public TestEquipmentTypesController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetEquipments endpoint returns a list of equipments.
    /// </summary>
    [Fact]
    public async Task GetEquipments_ReturnsEquipments()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<Equipment>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetEquipmentById endpoint returns the correct equipment.
    /// </summary>
    [Fact]
    public async Task GetEquipmentById_ReturnsEquipment()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);
       var result = await response.Content.ReadFromJsonAsync<Equipment>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(3);
    }
    
    /// <summary>
    /// Test to ensure that the CreateEquipment endpoint correctly creates an equipment and returns its Id.
    /// </summary>
    [Fact]
    public async Task CreateEquipment_ReturnsCreatedEquipment()
    {
        // Arrange
        // generate a random string 5 characters long
        var random = new Random();
        var equipment = new Equipment
        {
            Type = "Barbell*" + random.Next(1, 1000),
            Weight = 45,
            Resistance = 4,
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, equipment);
        HandleResponseFailure(response);
        var result = await response.Content.ReadFromJsonAsync<int>();
       
        // Assert
        result.Should().BeGreaterThan(0);
        
    }
    
    /// <summary>
    /// Test to ensure that the UpdateEquipment endpoint correctly updates an equipment.
    /// </summary>
    [Fact]
    public async Task UpdateEquipment_ReturnsUpdatedEquipment()
    {
        // Arrange
        var random = new Random();
        var equipment = new Equipment
        {
            Id = 2,
            Type = "Barbell" + random.Next(1, 10),
            Weight = 100,
            Resistance = 100
        };
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{equipment.Id}", equipment);
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteEquipment endpoint correctly deletes an equipment.
    /// </summary>
    [Fact]
    public async Task DeleteEquipment_ReturnsNoContent()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    
}