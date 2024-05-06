using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExercisesType.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the ExerciseTypeController.
/// </summary>
public class TestExerciseTypeController : BaseIntegrationTest
{
    
    const  string BaseUrl = "/api/exercisetypes";
    
    public TestExerciseTypeController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetExerciseTypes endpoint returns a list of ExerciseTypes.
    /// </summary>
    [Fact]
    public async Task GetExerciseTypes_ReturnsExerciseTypes()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<ExerciseType>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetExerciseTypeById endpoint returns the correct ExerciseType.
    /// </summary>
    [Fact]
    public async Task GetExerciseTypeById_ReturnsExerciseType()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);
        
       var result = await response.Content.ReadFromJsonAsync<ExerciseType>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(3);
    }
    
    /// <summary>
    /// Test to ensure that the CreateExerciseType endpoint correctly creates a new ExerciseType.
    /// </summary>
    [Fact]
    public async Task CreateExerciseType_ReturnsCreatedExerciseType()
    {
        // Arrange
        // generate a random string 5 characters long
        var random = new Random();
        
        var exerciseType = new ExerciseType
        {
            Name = "Test Exercise Type" + random.Next(10, 100).ToString(),
            Description = "Test Description",
            ImageUrl = "https://test.com/image.jpg"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, exerciseType);
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Test to ensure that the UpdateExerciseType endpoint correctly updates an existing ExerciseType.
    /// </summary>
    [Fact]
    public async Task UpdateExerciseType_ReturnsUpdatedExerciseType()
    {
        // Arrange
        // The id must be different that the one used in the delete test
        // otherwise sometimes when the delete test runs first it will delete the exercise type
        // and this test will fail
        var exerciseType = new ExerciseType
        {
            Id = 2,
            Name = "Test Exercise Type Updated",
            Description = "Test Description Updated",
            ImageUrl = "https://test.com/image.jpg"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", exerciseType);
        
        HandleResponseFailure(response);
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteExerciseType endpoint correctly deletes an existing ExerciseType.
    /// </summary>
    [Fact]
    public async Task DeleteExerciseType_ReturnsDeletedExerciseType()
    {
        // Arrange
        var id = 1;
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/{id}");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
}