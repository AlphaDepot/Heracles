using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the MuscleGroupController.
/// </summary>
public class TestMuscleGroupController : BaseIntegrationTest
{
    const string BaseUrl = "/api/musclegroup";
    
    public TestMuscleGroupController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetMuscleGroups endpoint returns a list of MuscleGroups.
    /// </summary>
    [Fact]
    public async Task GetMuscleGroups_ReturnsMuscleGroups()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<MuscleGroup>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetMuscleGroupById endpoint returns the correct MuscleGroup.
    /// </summary>
    [Fact]
    public async Task GetMuscleGroupById_ReturnsMuscleGroup()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);
        
       var result = await response.Content.ReadFromJsonAsync<MuscleGroup>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(3);
    }
    
    /// <summary>
    /// Test to ensure that the CreateMuscleGroup endpoint creates a new MuscleGroup and returns its Id.
    /// </summary>
    [Fact]
    public async Task CreateMuscleGroup_ReturnsCreatedMuscleGroup()
    {
        // Arrange
        var muscleGroup = new MuscleGroup
        {
            Name = "Test Muscle Group",
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, muscleGroup);

       HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    /// Test to ensure that the UpdateMuscleGroup endpoint updates a MuscleGroup correctly.
    /// </summary>
    [Fact]
    public async Task UpdateMuscleGroup_ReturnsUpdatedMuscleGroup()
    {
        // Arrange
        var muscleGroup = new MuscleGroup
        {
            Id = 2,
            Name = "Updated Muscle Group",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", muscleGroup);
        
        HandleResponseFailure(response);
        
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteMuscleGroup endpoint deletes a MuscleGroup correctly.
    /// </summary>
    [Fact]
    public async Task DeleteMuscleGroup_ReturnsDeletedMuscleGroup()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
}