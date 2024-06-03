using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;

/// <summary>
///   This class is used to test the UserExercisesController API.
/// </summary>
public class TestUserExercisesController : BaseIntegrationTest
{
    const string BaseUrl = "/api/userexercises";
    
    public TestUserExercisesController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    ///   This test checks if the GetUserExercises API returns the user exercises.
    /// </summary>
    [Fact]
    public async Task GetUserExercises_ReturnsUserExercises()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponseDto<UserExercise>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    ///   This test checks if the GetUserExerciseById API returns the user exercise with the specified ID.
    /// </summary>
    [Fact]
    public async Task GetUserExerciseById_ReturnsUserExercise()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/3");
        
        
        HandleResponseFailure(response);
        
       var result = await response.Content.ReadFromJsonAsync<UserExercise>();

        // Assert
       result.Should().NotBeNull();
       result.Id.Should().Be(3);
    }
    
    
    /// <summary>
    ///   This test checks if the CreateUserExercise API creates a user exercise and returns its ID.
    /// </summary>
    [Fact]
    public async Task CreateUserExercise_ReturnsCreatedUserExercise()
    {
        // Arrange
        var userExercise = new CreateUserExerciseDto
        {
            UserId = AdminUserId,
            ExerciseId = 2
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, userExercise);
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    /// This test will check creating a user exercise with the same exercise id and user id. to trigger the version increment.
    ///  </summary>
    [Fact]
    public async Task CreateUserExercise_ReturnsCreatedUserExerciseWithVersionIncrement()
    {
        // Arrange
        var userExercise = new CreateUserExerciseDto
        {
            UserId = AdminUserId,
            ExerciseId = 2
        };
        
        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, userExercise);
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<int>();
        
        // Assert
        result.Should().BeGreaterThan(0);
    }
    
    /// <summary>
    ///   This test checks if the UpdateUserExercise API updates a user exercise and returns a success status code.
    /// </summary>
    [Fact]
    public async Task UpdateUserExercise_ReturnsUpdatedUserExercise()
    {
        // Arrange
        var userExercise = new UpdateUserExerciseDto
        {
            Id = 2,
            UserId = AdminUserId,
            CurrentWeight = 50,

        };
        
        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", userExercise);
        
        HandleResponseFailure(response);
        
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    ///   This test checks if the DeleteUserExercise API deletes a user exercise and returns a success status code.
    /// </summary>
    [Fact]
    public async Task DeleteUserExercise_ReturnsDeletedUserExercise()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/1");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    
}