using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;
/// <summary>
/// This class contains integration tests for the EquipmentGroupsController.
/// </summary>
public class TestEquipmentGroupsController : BaseIntegrationTest
{
    const string BaseUrl = "/api/EquipmentGroups";
    public TestEquipmentGroupsController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetEquipmentGroups endpoint returns a list of equipment groups.
    /// </summary>
    [Fact]
    public async Task GetEquipmentGroups_ReturnsEquipmentGroups()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponseDto<EquipmentGroup>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetEquipmentGroupById endpoint returns the correct equipment group.
    /// </summary>
    [Fact]
    public async Task GetEquipmentGroupById_ReturnsEquipmentGroup()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        
        
        HandleResponseFailure(response);
       var result = await response.Content.ReadFromJsonAsync<EquipmentGroup>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(3);
    }
    
    /// <summary>
    /// Test to ensure that the CreateEquipmentGroup endpoint correctly creates a new equipment group.
    /// </summary>
    [Fact]
    public async Task CreateEquipmentGroup_ReturnsCreatedEquipmentGroup()
    {
        // Arrange
        var equipmentGroup = new EquipmentGroup
        {
            Name = "Test Equipment Group",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, equipmentGroup);

        HandleResponseFailure(response);
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    /// Test to ensure that the UpdateEquipmentGroup endpoint correctly updates an existing equipment group.
    /// </summary>
    [Fact]
    public async Task UpdateEquipmentGroup_ReturnsUpdatedEquipmentGroup()
    {
        // Arrange
        var equipmentGroup = new EquipmentGroup
        {
            Id = 2,
            Name = "Test Equipment Group Updated",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", equipmentGroup);
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();

    }
    
    /// <summary>
    /// Test to ensure that the DeleteEquipmentGroup endpoint correctly deletes an existing equipment group.
    /// </summary>
    [Fact]
    public async Task DeleteEquipmentGroup_ReturnsDeletedEquipmentGroup()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the AddEquipmentToGroup endpoint correctly adds an equipment to an existing group.
    /// </summary>
    [Fact]
    public async Task AddEquipmentToGroup_ReturnsEquipmentGroup()
    {
        // Arrange
        var equipmentGroup = new AddRemoveEquipmentGroupDto
        {
            EquipmentId = 1,
            EquipmentGroupId = 2
        };
        
        // Act
        var response = await _client.PatchAsJsonAsync($"{BaseUrl}/2/add", equipmentGroup);
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the RemoveEquipmentFromGroup endpoint correctly removes an equipment from an existing group.
    /// </summary>
    [Fact]
    public async Task RemoveEquipmentFromGroup_ReturnsEquipmentGroup()
    {
        // Arrange
        var equipmentGroup = new AddRemoveEquipmentGroupDto
        {
            EquipmentId = 1,
            EquipmentGroupId = 2
        };
        
        // Act
        var response = await _client.PatchAsJsonAsync($"{BaseUrl}/2/remove", equipmentGroup);
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    
}