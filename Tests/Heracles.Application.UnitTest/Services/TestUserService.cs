using System.Text.Json;
using FluentAssertions;
using Heracles.Application.Features.UserExercises;
using Heracles.Application.Features.Users;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Heracles.TestUtilities.TestData;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the UserService class.
/// </summary>
public class TestUserService : BaseUnitTest
{
    private readonly UserService _userService;
    private readonly Mock<IAppLogger<UserService>> _logger;
    
    /// <summary>
    /// Constructor for TestUserService, initializes mock objects and test data.
    /// </summary>
    public TestUserService(ITestOutputHelper testConsole) : base(testConsole)
    {
       _logger = new Mock<IAppLogger<UserService>>();
        _userService = new UserService(_logger.Object, UserRepository.Object, HttpContextAccessorWithAdminUser.Object);
    }
    
    
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_ReturnsUsers(QueryRequest query)
    {
        // Arrange
        SearchTerm = ValidAdminUserId;
        var expected = UserFixture.Query(query, ValidUserId, true);
        
        // Act
        var result = await _userService.GetAsync(query);

        TestConsole.WriteLine(JsonSerializer.Serialize(result));
        
        // Assert
        ExpectedQueryResult.Success(result, expected);
    }
    
    
    /// <summary>
    /// Test case for GetUserByUserIdAsync method when user id is valid.
    /// </summary>
    [Fact] 
    public async Task GetUserByUserIdAsync_WithValidUserId_ShouldReturnUser()
    {
        // Arrange
        var user = Users.First();
        
        // Act
        var result = await _userService.GetUserByUserIdAsync(user!.UserId);
        
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(user);
    }
    
    /// <summary>
    /// Test case for GetUserByUserIdAsync method when user id is invalid.
    /// </summary>
    [Fact]
    public async Task GetUserByUserIdAsync_WithInvalidUserId_ShouldReturnNotFound()
    {
        // Arrange
        var user = new User()
        {
            UserId = "test" // invalid user id
        };
        
        // Act
        var result = await _userService.GetUserByUserIdAsync(user.UserId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(result.Error);
        result.Error.Should().BeEquivalentTo(EntityErrorMessage<User>.NotFound(user.UserId));
    }



    [Fact]
    public async Task CreateUserAsync_WithValidUser_ShouldCreateUser()
    {
        // Arrange
        var newUser = new User
        {
            UserId = "unique-userid",
            Name = NotADuplicateName,
        };
        
        var users = UserExerciseSeedData.Users().FirstOrDefault(q => q.UserId == newUser.UserId);
        
        // Act
        var result  = await _userService.CreateUserAsync(newUser);
        
        // Assert
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Once);
    }

    
    [Fact]
    public async Task CreateUserAsync_WithDuplicateUser_ShouldLogWarningAndFail()
    {
        // Arrange
        var newUser = new User
        {
            UserId = ValidUserId,
            Name =  NotADuplicateName,
        };
        
        // Act
        var result = await _userService.CreateUserAsync(newUser);
        
        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Never);
    }
    

    /// <summary>
    /// Test case for CreateUserAsync method when user is invalid.
    /// </summary>
    [Theory]
    [MemberData(nameof(CreateBadData))]
    public async Task CreateUserAsync_WithInvalidUser_ShouldLogWarningAndFail(string userId)
    {
       
        // Arrange
        var newUser =  new User
        {
            UserId =userId
        };
        var newId = UserExerciseSeedData.Users().Count + 1;


        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Never);
        
    }
    
    
    /// <summary>
    /// Test case for UpdateUserAsync method when user is valid.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser()
    {
        // Arrange
        var user = Users.First();
        user!.Name =NotADuplicateName;
        
        
        // Act
        var result = await _userService.UpdateUserAsync(user!);
        
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Once);
    }
    
    
    
    /// <summary>
    /// Test case for UpdateUserAsync method when user is invalid.
    /// </summary>
    [Theory]
    [MemberData(nameof(UpdateInvalidData))]
    public async Task UpdateUserAsync_WithInvalidUser_ShouldLogWarning(User user)
    {
        // Act
        var result = await _userService.UpdateUserAsync(user);
        
        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Never);
    }

    
    [Fact]
    public async Task TestIsUserAuthorized_WhenUserIsNotAdminAndNotSameUser_ReturnsFalse()
    {
        // Arrange
        var userId =  ValidUserId;
        var currentUserId =SecondValidUserId;
        // Act
        var result = await _userService.IsUserAuthorized(userId, currentUserId);
        
        // Assert

       result.Should().BeFalse();
    }
    
    [Fact]
    public async Task TestIsUserAuthorized_WhenUserIsAdminAndNotSameUser_ReturnsTrue()
    {
        // Arrange
        var userId = ValidUserId;
        var currentUserId = ValidAdminUserId;
        
        // Act
        var result = await _userService.IsUserAuthorized(userId, currentUserId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task TestIsUserAuthorized_WhenUserIsNotAdminAndSameUser_ReturnsTrue()
    {
        // Arrange
        var userId = ValidUserId;
        var currentUserId = ValidUserId;

        // Act
        var result = await _userService.IsUserAuthorized(userId, currentUserId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task TestDoesUserExist_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var userId = ValidUserId;
        
        // Act
        var result = await _userService.DoesUserExist(userId);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task TestDoesUserExist_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = InvalidUserId;
        
        // Act
        var result = await _userService.DoesUserExist(userId);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteUserAsync_WithValidUserId_ShouldDeleteUser()
    {
        // Arrange
        var user = Users.First();
        
        // Act
        var result = await _userService.DeleteUserAsync(user!.UserId);
        
        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Never);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUserAsync_WithInvalidUserId_ShouldLogWarningAndFail()
    {
        // Arrange
        var user = new User()
        {
            UserId = "test" // invalid user id
        };
        
        // Act
        var result = await _userService.DeleteUserAsync(user.UserId);
        
        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        _logger.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once);
        _logger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Never);
    }


#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///  Provides user data for CreateUserAsync test case.
    /// </summary>
    /// <returns></returns>
    public static TheoryData<string> CreateBadData()
    {

        return new TheoryData<string>
        {
            null,
            InvalidUserId,
            new string('a', 256)
        };
    }
        

    /// <summary>
    /// Provides invalid user data for UpdateUserAsync test case.
    /// </summary>
    /// <returns> Invalid user data.</returns>
    public static TheoryData<User> UpdateInvalidData()
    {
        return new TheoryData<User>
        {
            new User {Id= 0 , UserId = "test-userid-greater-than-2-but-less-than-256-characters"}, // invalid id
            new User {Id=-1 , UserId = "test-userid-greater-than-2-but-less-than-256-characters"}, // invalid id
            new User {Id= 1 , UserId = "i"} // invalid user id
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}