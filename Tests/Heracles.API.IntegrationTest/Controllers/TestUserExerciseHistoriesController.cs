using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExerciseHistories.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;

/// <summary>
///  This class is used to test the UserExerciseHistoriesController.
/// </summary>
public class TestUserExerciseHistoriesController : BaseIntegrationTest
{
    const string BaseUrl = "/api/userexercisehistories";
    public TestUserExerciseHistoriesController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    ///  Test case for getting all user exercise histories.
    /// </summary>
    [Fact]
    public async Task GetUserExerciseHistories_ReturnsUserExerciseHistories()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponse<UserExerciseHistory>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    ///  Test case for getting a specific user exercise history by ID.
    /// </summary>
    [Fact]
    public async Task GetUserExerciseHistoryById_ReturnsUser() 
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<UserExerciseHistory>();
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(3);
    }
    
    
    /// <summary>
    ///  Test case for creating a new user exercise history.
    /// </summary>
    [Fact]
    public async Task CreateUserExerciseHistory_ReturnsCreatedUserExerciseHistory()
    {
        // Arrange
        var userExerciseHistory = new UserExerciseHistory
        {
            UserId = AdminUserId,
            UserExerciseId = 2,
            Weight = 50,
            Repetition = 10
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, userExerciseHistory);
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    ///  Test case for updating an existing user exercise history.
    /// </summary>
    [Fact]
    public async Task UpdateUserExerciseHistory_ReturnsUpdatedUserExerciseHistory()
    {
        // Arrange
        var userExerciseHistory = new UserExerciseHistory
        {
            Id = 2,
            UserId = AdminUserId,
            UserExerciseId = 2,
            Weight = 70,
            Repetition = 10
        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/1", userExerciseHistory);
        
        HandleResponseFailure(response);
        

        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    ///  Test case for deleting a user exercise history.
    /// </summary>
    [Fact]
    public async Task DeleteUserExerciseHistory_ReturnsDeletedUserExerciseHistory()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    
    
}