using Heracles.Application.Features.UserExercisesHistories;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
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
    /// Test for the GetAsync method with filter and sort parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering and sorting.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData(QueryRequest query)
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
    /// Test for the GetByIdAsync method.
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
        if (expected == TestDomainResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, history!, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.ValidationFail(_logger, result, result.Error.Errors!);

    }

    /// <summary>
    /// Test for the CreateAsync method.
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
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, history.Id,
                result.Value);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                result.Error.Errors!);
    }
    /// <summary>
    /// Test for the UpdateAsync method.
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
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                newHistory.Id);
        
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<UserExerciseHistoryService, UserExerciseHistory>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    /// Test for the DeleteAsync method.
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
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, history!.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.ValidationFail<UserExerciseHistoryService, UserExerciseHistory>(_logger, result, result.Error.Errors!);
    }


    

    /// <summary>
    ///  Test for the CreateAsync method.
    /// </summary>
    /// <returns> Task</returns>
    public static IEnumerable<object?[]> CreateData()
    {
        var validUserExerciseId = 1;
        var validDate = new DateTime(2022, 1, 1);


        yield return new object[] {ValidAdminUserId, validUserExerciseId, 5, 100, validDate, TestDomainResponse.Success }; // valid data
        yield return new object[] {ValidUserId, validUserExerciseId, 5, 100, validDate, TestDomainResponse.Success }; // valid data
        yield return new object?[] {ValidAdminUserId,  null, 5, 100, validDate, TestDomainResponse.BadRequest }; // invalid user exercise id
        yield return new object?[] {ValidAdminUserId,  0, 5, 100, validDate, TestDomainResponse.BadRequest }; // invalid user exercise id
        yield return new object?[]
            {ValidAdminUserId,  validUserExerciseId, null, 100, validDate, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[]
            {ValidAdminUserId,  validUserExerciseId, -1, 100, validDate, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[]
            { ValidAdminUserId, validUserExerciseId, 0, 100, validDate, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[]
            {ValidAdminUserId,  validUserExerciseId, 5, null, validDate, TestDomainResponse.BadRequest }; // invalid weight
        yield return new object?[] {ValidAdminUserId,  validUserExerciseId, 5, -1, validDate, TestDomainResponse.BadRequest }; // invalid weight
        yield return new object?[] {ValidAdminUserId,  validUserExerciseId, 5, 0, validDate, TestDomainResponse.BadRequest }; // invalid weight
        
        yield return new object?[] {InvalidUserId, validUserExerciseId, 5, 100, validDate, TestDomainResponse.BadRequest }; // invalid user id
    }


    /// <summary>
    ///  Test for the UpdateAsync method.
    /// </summary>
    /// <returns> Task</returns>
    public static IEnumerable<object?[]> UpdateData()
    {
        yield return new object[] { 1,ValidAdminUserId, 56, 150, TestDomainResponse.Success }; // valid data
        yield return new object[] { 1,ValidAdminUserId, 56, 150, TestDomainResponse.Success }; // valid data
//yield return new object?[] { 1,_validUserId, 5, 100, TestDomainResponse.BadRequest }; // invalid user id
        
        yield return new object[] { 0,ValidAdminUserId, 5, 100, TestDomainResponse.BadRequest }; // invalid id
        yield return new object?[] { null,ValidAdminUserId, 5, 100, TestDomainResponse.BadRequest }; // invalid id
        yield return new object[] { -1,ValidAdminUserId, 5, 100, TestDomainResponse.BadRequest }; // invalid id
        yield return new object?[] { 1,ValidAdminUserId, null, 100, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[] { 1,ValidAdminUserId, -1, 100, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[] { 1,ValidAdminUserId, 0, 100, TestDomainResponse.BadRequest }; // invalid repetition
        yield return new object?[] { 1,ValidAdminUserId, 5, null, TestDomainResponse.BadRequest }; // invalid weight
        yield return new object?[] { 1,ValidAdminUserId, 5, -1, TestDomainResponse.BadRequest }; // invalid weight
        yield return new object?[] { 1,ValidAdminUserId, 5, 0, TestDomainResponse.BadRequest }; // invalid weight
        
        yield return new object?[] { 1,null, 5, 100, TestDomainResponse.BadRequest }; // invalid user id
        yield return new object?[] { 1,InvalidUserId, 5, 100, TestDomainResponse.BadRequest }; // invalid user id
    }
}