using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleFunctions.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;
/// <summary>
/// This class contains integration tests for the MuscleFunctionsController.
/// </summary>
public class TestMuscleFunctionsController : BaseIntegrationTest
{
    
    const  string BaseUrl = "/api/MuscleFunctions";
    
    public TestMuscleFunctionsController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetMuscleFunctions endpoint returns a list of MuscleFunctions.
    /// </summary>
    [Fact]
    public async Task GetMuscleFunctions_ReturnsMuscleFunctions()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<MuscleFunction>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetMuscleFunctionById endpoint returns a specific MuscleFunction.
    /// </summary>
    [Fact]
    public async Task GetMuscleFunctionById_ReturnsMuscleFunction()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/2");
        HandleResponseFailure(response);
        
       var result = await response.Content.ReadFromJsonAsync<MuscleFunction>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(2);
    }
    
    /// <summary>
    /// Test to ensure that the CreateMuscleFunction endpoint creates a new MuscleFunction.
    /// </summary>
    [Fact]
    public async Task CreateMuscleFunction_ReturnsCreatedMuscleFunction()
    {
        // Arrange
        var random = new Random();
        var muscleFunction = new MuscleFunction
        {
            Name = "Test Muscle Function" + random.Next(1, 10),
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, muscleFunction);

        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    /// Test to ensure that the UpdateMuscleFunction endpoint updates a specific MuscleFunction.
    /// </summary>
    [Fact]
    public async Task UpdateMuscleFunction_ReturnsUpdatedMuscleFunction()
    {
        // Arrange
        var muscleFunction = new MuscleFunction
        {
            Id =2,
            Name = "Test Muscle Function",
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", muscleFunction);
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteMuscleFunction endpoint deletes a specific MuscleFunction.
    /// </summary>
    [Fact]
    public async Task DeleteMuscleFunction_ReturnsDeletedMuscleFunction()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/3");
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    
}