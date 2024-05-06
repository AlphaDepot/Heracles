using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;
/// <summary>
/// This class contains integration tests for the ExerciseMuscleGroupController.
/// </summary>
public class TestExerciseMuscleGroupController : BaseIntegrationTest
{
    const string BaseUrl = "/api/exercisemusclegroup";
    public TestExerciseMuscleGroupController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetExerciseMuscleGroups API endpoint returns a list of ExerciseMuscleGroups.
    /// </summary>
    [Fact]
    public async Task GetExerciseMuscleGroups_ReturnsExerciseMuscleGroups()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<ExerciseMuscleGroup>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetExerciseMuscleGroupById API endpoint returns the correct ExerciseMuscleGroup.
    /// </summary>
    [Fact]
    public async Task GetExerciseMuscleGroupById_ReturnsExerciseMuscleGroup()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<ExerciseMuscleGroup>();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(3);
    }

    /// <summary>
    /// Test to ensure that the GetExerciseMuscleGroupByExerciseId API endpoint returns a list of ExerciseMuscleGroups.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result is a list of ExerciseMuscleGroups.</returns>
    /// <param name="exerciseId">The ID of the exercise.</param>
    [Fact]
    public async Task GetExerciseMuscleGroupByExerciseId_ReturnsExerciseMuscleGroups()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/exercise/2");
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<ExerciseMuscleGroup>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
        
        
        
    /// <summary>
    /// Test to ensure that the CreateExerciseMuscleGroup API endpoint correctly creates an ExerciseMuscleGroup.
    /// </summary>
    [Fact]
    public async Task CreateExerciseMuscleGroup_ReturnsCreatedExerciseMuscleGroup()
    {
        // Arrange
        var exerciseMuscleGroup = new CreateExerciseMuscleGroupDto
        {
            ExerciseTypeId = 2,
            MuscleGroupId = 3,
            MuscleFunctionId = 3,
            FunctionPercentage = 90
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, exerciseMuscleGroup);
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
       
    }
    
    /// <summary>
    /// Test to ensure that the UpdateExerciseMuscleGroup API endpoint correctly updates an ExerciseMuscleGroup.
    /// </summary>
    [Fact]
    public async Task UpdateExerciseMuscleGroup_ReturnsUpdatedExerciseMuscleGroup()
    {
        // Arrange
        var exerciseMuscleGroup = new UpdateExerciseMuscleGroupDto
        {
           Id = 1,
           FunctionPercentage = 98
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{exerciseMuscleGroup.Id}", exerciseMuscleGroup);
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteExerciseMuscleGroup API endpoint correctly deletes an ExerciseMuscleGroup.
    /// </summary>
    [Fact]
    public async Task DeleteExerciseMuscleGroup_ReturnsNoContent()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
}