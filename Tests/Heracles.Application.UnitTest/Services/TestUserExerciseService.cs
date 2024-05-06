using Heracles.Application.Features.UserExercises;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the UserExerciseService class.
/// </summary>
public class TestUserExerciseService : BaseUnitTest
{
    // Single service mock instance
    private readonly Mock<IAppLogger<UserExerciseService>> _logger;
    // Multiple service mock instances
    private readonly UserExerciseService _serviceWithAdminUser;
    private readonly UserExerciseService _serviceWithNormalUser;

    /// <summary>
    /// Constructor for TestUserExerciseService, initializes mock objects and test data.
    /// </summary>
    public TestUserExerciseService(ITestOutputHelper testConsole) : base(testConsole)
    {
        // set up the mock objects
        _logger = new Mock<IAppLogger<UserExerciseService>>();
        
        // set up the service with the mock objects for admin and non admin user
        _serviceWithAdminUser = new UserExerciseService(_logger.Object, HttpContextAccessorWithAdminUser.Object, UserExerciseRepository.Object, ExerciseTypeRepository.Object,
            EquipmentGroupRepository.Object, UserService.Object);
        _serviceWithNormalUser = new UserExerciseService(_logger.Object, HttpContextAccessorWithNormalUser.Object, UserExerciseRepository.Object, ExerciseTypeRepository.Object,
            EquipmentGroupRepository.Object,  UserService.Object);
    }

    /// <summary>
    /// Test case for GetAsync method with filter and sort parameters when user is admin.
    /// </summary>
    ///  <param name="query">The query request to get user exercises.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsAdmin(QueryRequest query)
    {
        // Arrange
        var expected = UserExerciseFixture.Query(query, ValidAdminUserId, true);

        // Act
        var result = await _serviceWithAdminUser.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///  Test case for GetAsync method with filter and sort parameters when user is not admin.
    /// </summary>
    /// <param name="query">The query request to get user exercises.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsNotAdmin(QueryRequest query)
    {
        // Arrange
        SearchTerm = "Bench Press";
        // set up the user id
        var expected = UserExerciseFixture.Query(query, ValidUserId, false);

        // Act
        var result = await _serviceWithNormalUser.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }
    
    /// <summary>
    /// Test case for GetByIdAsync method.
    /// </summary>
    ///  <param name="id">The id of the user exercise to get.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncFluentData))]
    public async Task GetByIdAsync_WithId_ReturnsData(int id, string expected)
    {
        // Arrange
        var userExercise = UserExercises.FirstOrDefault(x => x.Id == id);

        // Act
        var result = await _serviceWithAdminUser.GetByIdAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, userExercise!, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.ValidationFail(_logger, result, result.Error.Errors!);

    }
    
    
    
    /// <summary>
    /// Test case for CreateAsync method.
    /// </summary>
    ///  <param name="userId">The user id of the user exercise to create.</param>
    ///  <param name="exerciseId">The exercise id of the user exercise to create.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string userId, int exerciseId, string expected)
    {
        // Arrange
        // create a new id based on the count of the user exercises
        var expectedId = UserExercises.Count + 1;

        var dto = new CreateUserExerciseDto
        {
            UserId = userId,
            ExerciseId = exerciseId
        };

        // Act
        var result = await _serviceWithAdminUser.CreateAsync(dto);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<UserExerciseService, UserExercise>(_logger, result, expectedId, result.Value);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }
    
    
    
    /// <summary>
    /// Test case for UpdateAsync method.
    /// </summary>
    ///  <param name="id">The id of the user exercise to update.</param>
    ///  <param name="userId">The user id of the user exercise to update.</param>
    ///  <param name="currentWeight">The current weight of the user exercise to update.</param>
    ///  <param name="personalRecord">The personal record of the user exercise to update.</param>
    ///  <param name="durationInSeconds">The duration in seconds of the user exercise to update.</param>
    ///  <param name="sortOrder">The sort order of the user exercise to update.</param>
    ///  <param name="repetitions">The repetitions of the user exercise to update.</param>
    ///  <param name="sets">The sets of the user exercise to update.</param>
    ///  <param name="timed">The timed of the user exercise to update.</param>
    ///  <param name="bodyWeight">The body weight of the user exercise to update.</param>
    ///  <param name="equipmentGroupId">The equipment group id of the user exercise to update.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string userId, double? currentWeight, double? personalRecord,
        int durationInSeconds, int sortOrder, int repetitions, int sets, bool timed, bool bodyWeight,
        int equipmentGroupId, string expected)
    {
        // Arrange
        // get the list of equipment groups
        var equipmentGroup = EquipmentGroupFixture.Get();
        // get the user exercise by id or create a new one
        var userExercise = UserExercises.FirstOrDefault(x => x.Id == id)
                           ?? new UserExercise
                           {
                               Id = id,
                               UserId = userId,
                               CurrentWeight = currentWeight,
                               PersonalRecord = personalRecord,
                               DurationInSeconds = durationInSeconds,
                               SortOrder = sortOrder,
                               Repetitions = repetitions,
                               Sets = sets,
                               Timed = timed,
                               BodyWeight = bodyWeight,
                               EquipmentGroup = equipmentGroup.First()
                           };


        // create the dto
        var dto = new UpdateUserExerciseDto
        {
            Id = id,
            UserId = userId,
            CurrentWeight = currentWeight,
            PersonalRecord = personalRecord,
            DurationInSeconds = durationInSeconds,
            SortOrder = sortOrder,
            Repetitions = repetitions,
            Sets = sets,
            Timed = timed,
            BodyWeight = bodyWeight,
            EquipmentGroupId = equipmentGroupId
        };

        // Act
        var result = await _serviceWithAdminUser.UpdateAsync(dto);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<UserExerciseService, UserExercise>(_logger, result, userExercise.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }
    /// <summary>
    /// Test case for DeleteAsync method.
    /// </summary>
    ///  <param name="id">The id of the user exercise to delete.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncFluentData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var userExercise = UserExercises.FirstOrDefault(x => x.Id == id);

        // Act
        var result = await _serviceWithAdminUser.DeleteAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<UserExerciseService, UserExercise>(_logger, result, userExercise!.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.ValidationFail<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }
    
    
    /// <summary>
    ///  Test case for CreateAsync method.
    /// </summary>
    /// <returns>   Task </returns>
    public static IEnumerable<object?[]> CreateData()
    {
        yield return new object[] { ValidAdminUserId, 1, TestDomainResponse.Success };
        yield return new object?[] { null, null, TestDomainResponse.BadRequest };
        yield return new object[] { null, 1, TestDomainResponse.BadRequest };
        yield return new object?[] { ValidAdminUserId, null, TestDomainResponse.BadRequest };
        yield return new object?[] { ValidAdminUserId, 0, TestDomainResponse.BadRequest };
        
        yield return new object[] {InvalidUserId, 1, TestDomainResponse.BadRequest };
    }

    /// <summary>
    ///  Test case for UpdateAsync method.
    /// </summary>
    /// <returns>   Task </returns>
    public static IEnumerable<object?[]> UpdateData()
    {
        //NOTE: cannot validate most fields  for null values because they are optional in the dto
        
        // id, userId, currentWeight, personalRecord, durationInSeconds, sortOrder, repetitions, sets, timed, bodyWeight, equipmentGroupId
        yield return new object[] { 1, ValidAdminUserId, 100.0, 200.0, 60, 1, 10, 3, true, false, 1, TestDomainResponse.Success };

        yield return new object?[]
            { 0, ValidAdminUserId, 100.0, 200.0, 60, 1, 10, 3, true, false, 1, TestDomainResponse.BadRequest }; // id
        yield return new object?[]
            { 1, null, 100.0, 200.0, 60, 1, 10, 3, true, false, 1, TestDomainResponse.BadRequest }; // userId

        yield return new object[]
        {
            1, ValidAdminUserId, 100.0, 200.0, -1, 1, 10, 3, true, false, 1, TestDomainResponse.BadRequest
        }; // durationInSeconds
        yield return new object[]
            { 1, ValidAdminUserId, 100.0, 200.0, 60, 1, -1, 3, true, false, 1, TestDomainResponse.BadRequest }; // repetitions
        yield return new object[]
            { 1, ValidAdminUserId, 100.0, 200.0, 60, 1, 10, -1, true, false, 1, TestDomainResponse.BadRequest }; // sets
        yield return new object[]
        {
            1, ValidAdminUserId, 100.0, 200.0, 60, 1, 10, 3, true, false, -1, TestDomainResponse.BadRequest
        }; // equipmentGroupId
        
        yield return new object[] {1,InvalidUserId, 100.0, 200.0, 60, 1, 10, 3, true, false, -1, TestDomainResponse.BadRequest
        };  // invalid user id
    }
}