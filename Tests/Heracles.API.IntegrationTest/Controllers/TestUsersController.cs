using System.Net.Http.Json;
using FluentAssertions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Users.Models;
using Xunit.Abstractions;

namespace Heracles.API.IntegrationTest.Controllers;


/// <summary>
/// This class is used to test the functionality of the Users API controller.
/// </summary>
public class TestUsersController : BaseIntegrationTest
{
    const string BaseUrl = "/api/users";
    public TestUsersController(HeraclesWebApplicationFactory factory, ITestOutputHelper console) : base(factory, console)
    {
    }
    
    /// <summary>
    /// Test to ensure that the GetUsers endpoint returns a list of users.
    /// </summary>
    [Fact]
    public async Task GetUsers_ReturnsUsers()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync(BaseUrl);
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<QueryResponseDto<User>>();
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
    }
    
    /// <summary>
    /// Test to ensure that the GetUserById endpoint returns the correct user.
    /// </summary>
    [Fact]
    public async Task GetUserById_ReturnsUser() 
    {
        // Arrange
        // Act
        var response = await _client.GetAsync($"{BaseUrl}/{AdminUserId}");
        
        
        HandleResponseFailure(response);
        
        var result = await response.Content.ReadFromJsonAsync<User>();
        
        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(AdminUserId);
    }

    /// <summary>
    /// Test to ensure that the CreateUser endpoint correctly creates a user.
    /// </summary>
    [Fact]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User
        {

            UserId = "ddsdfsdfsd",
            Name = "John",
            Email = " test@jotest.com",

        };

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, user);

        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        result.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Test to ensure that the UpdateUser endpoint correctly updates a user.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {
        // Arrange
        var user = new User
        {
            Id = 2,
            UserId = NonAdminUserId,
            Name = "John",
            Email = "john@outlook.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"{BaseUrl}/2", user);
        
        
        
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// Test to ensure that the DeleteUser endpoint correctly deletes a user.
    /// </summary>
    [Fact]
    public async Task DeleteUser_ReturnsDeletedUser()
    {
        // Arrange
        // Act
        var response = await _client.DeleteAsync($"{BaseUrl}/{AdminUserId}");
        
        HandleResponseFailure(response);
        
        // Assert
        response.EnsureSuccessStatusCode();
    }




}