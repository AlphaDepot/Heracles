using Heracles.Application.Features.UserExercisesHistories;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;
/// <summary>
/// This class contains unit tests for the UserExerciseHistoryService.
/// </summary>
public class TestUserExerciseHistoryService : BaseUnitTest
{
    private readonly Mock<IAppLogger<UserExerciseHistoryService>> _logger;
    private readonly UserExerciseHistoryService _serviceWithAdminUser;
    private readonly UserExerciseHistoryService _serviceWithNormalUser;
    
    /// <summary>
    /// Constructor for the TestUserExerciseHistoryService class.
    /// </summary>
    public TestUserExerciseHistoryService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<UserExerciseHistoryService>>();
        
        _serviceWithAdminUser = new UserExerciseHistoryService(_logger.Object,HttpContextAccessorWithAdminUser.Object ,UserExerciseHistoryRepository.Object, UserExerciseRepository.Object, UserService.Object);
        _serviceWithNormalUser = new UserExerciseHistoryService(_logger.Object,HttpContextAccessorWithNormalUser.Object ,UserExerciseHistoryRepository.Object, UserExerciseRepository.Object, UserService.Object);
    }
    
    /// <summary>
    /// Test for the GetAllPagedAsync method with filter and sort parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering and sorting.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData(QueryRequestDto query)
    {
        // Arrange
        SearchTerm = "1";
        var expected = UserExerciseHistoryFixture.Query(query, ValidAdminUserId, true);

        // Act
        var result = await _serviceWithAdminUser.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }
    /// <summary>
    /// Test for the GetEntityByIdAsync method.
    /// </summary>
    /// <param name="id">The id of the UserExerciseHistory to retrieve.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncFluentData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        var history = UserExerciseHistory.FirstOrDefault(x => x.Id == id);

        // Act
        var result = await _serviceWithAdminUser.GetByIdAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, history!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.ValidationFail(_logger, result, result.Error.Errors!);

    }

    /// <summary>
    /// Test for the CreateEntityAsync method.
    /// </summary>
    /// <param name="userExerciseId">The id of the UserExercise to associate with the UserExerciseHistory.</param>
    /// <param name="repetition">The number of repetitions for the UserExerciseHistory.</param>
    /// <param name="weight">The weight for the UserExerciseHistory.</param>
    /// <param name="change">The date of the change for the UserExerciseHistory.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string userId, int userExerciseId, int repetition, int weight, DateTime change,
        string expected)
    {
        // Arrange
        var history = new UserExerciseHistory
        {
            UserExerciseId = userExerciseId,
            Repetition = repetition,
            Weight = weight,
            Change = change,
            UserId = userId ?? UserFixture.Get().First().UserId
        };

        // Act
        var result = await _serviceWithAdminUser.CreateAsync(history);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, history.Id,
                result.Value);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                result.Error.Errors!);
    }
    /// <summary>
    /// Test for the UpdateEntityAsync method.
    /// </summary>
    /// <param name="id">The id of the UserExerciseHistory to update.</param>
    ///  <param name="userId">The id of the User to update.</param>
    /// <param name="repetition">The new number of repetitions for the UserExerciseHistory.</param>
    /// <param name="weight">The new weight for the UserExerciseHistory.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string userId, int repetition, int weight, string expected)
    {
        // Arrange
        var newHistory = new UpdateUserExerciseHistoryDto
        {
            Id = id,
            Repetition = repetition,
            Weight = weight,
            UserId = userId
        };

        // Act
        var result = await _serviceWithAdminUser.UpdateAsync(newHistory);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                newHistory.Id);
        
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    /// Test for the DeleteEntityAsync method.
    /// </summary>
    /// <param name="id">The id of the UserExerciseHistory to delete.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncFluentData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var history = UserExerciseHistory.FirstOrDefault(x => x.Id == id);

        // Act
        var result = await _serviceWithAdminUser.DeleteAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, history!.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.ValidationFail<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, result.Error.Errors!);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///  Test for the CreateEntityAsync method.
    /// </summary>
    /// <returns> TheoryData</returns>
    public static TheoryData<string, int, int, int, DateTime, string> CreateData()
    {
        var validUserExerciseId = 1;
        var validDate = new DateTime(2022, 1, 1);

        return new TheoryData<string, int, int, int, DateTime, string>()
        {
            { ValidAdminUserId, validUserExerciseId, 5, 100, validDate, TestServiceResponse.Success }, // valid data
            { ValidUserId, validUserExerciseId, 5, 100, validDate, TestServiceResponse.Success }, // valid data
            { ValidAdminUserId,  0, 5, 100, validDate, TestServiceResponse.BadRequest }, // invalid user exercise id
            { ValidAdminUserId,  validUserExerciseId, -1, 100, validDate, TestServiceResponse.BadRequest }, // invalid repetition
            { ValidAdminUserId, validUserExerciseId, 0, 100, validDate, TestServiceResponse.BadRequest }, // invalid repetition
            { ValidAdminUserId,  validUserExerciseId, 5, -1, validDate, TestServiceResponse.BadRequest }, // invalid weight
            { ValidAdminUserId,  validUserExerciseId, 5, 0, validDate, TestServiceResponse.BadRequest }, // invalid weight
            { InvalidUserId, validUserExerciseId, 5, 100, validDate, TestServiceResponse.BadRequest } // invalid user id
        };
    }


    /// <summary>
    ///  Test for the UpdateEntityAsync method.
    /// </summary>
    /// <returns> TheoryData</returns>
    public static TheoryData<int, string, int, int, string> UpdateData()
    {
        return new TheoryData<int, string, int, int, string>()
        {
            { 1, ValidAdminUserId, 56, 150, TestServiceResponse.Success }, // valid data
            { 1, ValidAdminUserId, 56, 150, TestServiceResponse.Success }, // valid data
            { 0, ValidAdminUserId, 5, 100, TestServiceResponse.BadRequest }, // invalid id
            { -1, ValidAdminUserId, 5, 100, TestServiceResponse.BadRequest }, // invalid id
            { 1, ValidAdminUserId, -1, 100, TestServiceResponse.BadRequest }, // invalid repetition
            { 1, ValidAdminUserId, 0, 100, TestServiceResponse.BadRequest }, // invalid repetition
            { 1, ValidAdminUserId, 5, -1, TestServiceResponse.BadRequest }, // invalid weight
            { 1, ValidAdminUserId, 5, 0, TestServiceResponse.BadRequest }, // invalid weight
            { 1, null, 5, 100, TestServiceResponse.BadRequest }, // invalid user id
            { 1, InvalidUserId, 5, 100, TestServiceResponse.BadRequest } // invalid user id
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}