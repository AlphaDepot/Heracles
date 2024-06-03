using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;

/// <summary>
/// Represents a controller for testing workout sessions.
/// </summary>
public class TestWorkoutSessionsController : BaseIntegrationTest
{
    const string BaseUrl = "/api/workoutsessions";
    
    public TestWorkoutSessionsController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetWorkoutSessions endpoint returns workout sessions.
    /// </summary>
    [Fact]
    public async Task GetWorkoutSessions_ReturnsWorkoutSessions()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponseDto<WorkoutSession>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetWorkoutSessionById endpoint returns the correct workout session.
    /// </summary>
    [Fact]
    public async Task GetWorkoutSessionById_ReturnsWorkoutSession()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<WorkoutSession>();
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(3);
    }
    
    /// <summary>
    /// Test to ensure that the CreateWorkoutSession endpoint creates a new workout session.
    /// </summary>
    [Fact]
    public async Task CreateWorkoutSession_ReturnsCreatedWorkoutSession()
    {
        // Arrange
        var workoutSession = new WorkoutSession
        {
            UserId = AdminUserId,
            Name = "Test Workout Session NOT DUPLICATE",
            DayOfWeek = DayOfWeek.Friday
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, workoutSession);
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    /// Test to ensure that the UpdateWorkoutSession endpoint updates a workout session correctly.
    /// </summary>
    [Fact]
    public async Task UpdateWorkoutSession_ReturnsUpdatedWorkoutSession()
    {
        // Arrange
        var workoutSession = new WorkoutSession
        {
            Id = 2,
            UserId = AdminUserId,
            Name = "Test Workout Session",
            DayOfWeek = DayOfWeek.Friday
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/{workoutSession.Id}", workoutSession);
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the AddExerciseToWorkoutSession endpoint adds an exercise to a workout session correctly.
    /// </summary>
    [Fact]
    public async Task AddExerciseToWorkoutSession_ReturnsUpdatedWorkoutSession()
    {
        // Arrange
        var workoutSessionExercise = new WorkoutSessionExerciseDto
        {
            UserId = AdminUserId,
            WorkoutSessionId = 2,
            UserExerciseId = 2,
        };
        
        // Act
        var response = await _client.PatchAsJsonAsync($"{BaseUrl}/{workoutSessionExercise.WorkoutSessionId}/add", workoutSessionExercise);
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the RemoveExerciseFromWorkoutSession endpoint removes an exercise from a workout session correctly.
    /// </summary>
    [Fact]
    public async Task RemoveExerciseFromWorkoutSession_ReturnsUpdatedWorkoutSession()
    {
        // Arrange
        var workoutSessionExercise = new WorkoutSessionExerciseDto
        {
            UserId = AdminUserId,
            WorkoutSessionId = 2,
            UserExerciseId = 2,
        };
        
        // Act
        var response = await _client.PatchAsJsonAsync($"{BaseUrl}/{workoutSessionExercise.WorkoutSessionId}/remove", workoutSessionExercise);
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteWorkoutSession endpoint deletes a workout session correctly.
    /// </summary>
    [Fact]
    public async Task DeleteWorkoutSession_ReturnsDeletedWorkoutSession()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    
}