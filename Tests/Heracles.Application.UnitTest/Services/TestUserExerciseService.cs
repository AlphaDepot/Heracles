using Heracles.Application.Features.UserExercises;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.EquipmentGroups.Models;
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
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsAdmin(QueryRequestDto query)
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
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsNotAdmin(QueryRequestDto query)
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
    /// Test case for GetEntityByIdAsync method.
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
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, userExercise!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.ValidationFail(_logger, result, result.Error.Errors!);

    }
    
    
    
    /// <summary>
    /// Test case for CreateEntityAsync method.
    /// </summary>
    ///  <param name="userId">The user id of the user exercise to create.</param>
    ///  <param name="exerciseId">The exercise id of the user exercise to create.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string? userId, int exerciseId, string expected)
    {
        // Arrange
        // create a new id based on the count of the user exercises
        var expectedId = UserExercises.Count + 1;

        var dto = new CreateUserExerciseDto
        {
            UserId = userId ?? "",
            ExerciseId = exerciseId
        };

        // Act
        var result = await _serviceWithAdminUser.CreateAsync(dto);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<UserExerciseService, UserExercise>(_logger, result, expectedId, result.Value);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }



    /// <summary>
    /// Test case for UpdateEntityAsync method.
    /// </summary>
    ///  <param name="exercise"> The user exercise to update.</param>
    ///  <param name="equipmentGroupId">The equipment group id of the user exercise to update.</param>
    ///  <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(UserExercise exercise, int equipmentGroupId, string expected)
    {
        // Arrange
        // get the list of equipment groups
        var equipmentGroup = EquipmentGroupFixture.Get();
        // get the user exercise by id or create a new one
        var userExercise = UserExercises.FirstOrDefault(x => x.Id == exercise.Id)
                           ?? new UserExercise()
                           {
                               Id = exercise.Id,
                                 UserId = exercise.UserId,
                                 CurrentWeight = exercise.CurrentWeight,
                                 PersonalRecord = exercise.PersonalRecord,
                                 DurationInSeconds = exercise.DurationInSeconds,
                                 SortOrder = exercise.SortOrder,
                                 Repetitions = exercise.Repetitions,
                                 Sets = exercise.Sets,
                                 Timed = exercise.Timed,
                                 BodyWeight = exercise.BodyWeight,
                                 EquipmentGroup = equipmentGroup.First()
                           };


        // create the dto
        var dto = new UpdateUserExerciseDto
        {
            Id = userExercise.Id, 
            UserId = exercise.UserId, 
            CurrentWeight = exercise.CurrentWeight, 
            PersonalRecord = exercise.PersonalRecord, 
            DurationInSeconds = exercise.DurationInSeconds, 
            SortOrder = exercise.SortOrder, 
            Repetitions = exercise.Repetitions, 
            Sets = exercise.Sets, 
            Timed = exercise.Timed, 
            BodyWeight = exercise.BodyWeight, 
            EquipmentGroupId = equipmentGroupId
        };
        

        // Act
        var result = await _serviceWithAdminUser.UpdateAsync(dto);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<UserExerciseService, UserExercise>(_logger, result, userExercise.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    /// Test case for DeleteEntityAsync method.
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
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<UserExerciseService, UserExercise>(_logger, result, userExercise!.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.ValidationFail<UserExerciseService, UserExercise>(_logger, result, result.Error.Errors!);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///  Test case for CreateEntityAsync method.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<string?, int, string> CreateData()
    {
        return new TheoryData<string?, int, string>()
        {
            {ValidAdminUserId, 1, TestServiceResponse.Success},
            {null, 0, TestServiceResponse.BadRequest},
            {null, 1, TestServiceResponse.BadRequest},
            {ValidAdminUserId, 0, TestServiceResponse.BadRequest},
            {ValidAdminUserId, 0, TestServiceResponse.BadRequest},
            {InvalidUserId, 1, TestServiceResponse.BadRequest}
        };
    }

    /// <summary>
    ///  Test case for UpdateEntityAsync method.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<UserExercise, int, string> UpdateData()
    {
        //NOTE: cannot validate most fields  for null values because they are optional in the dto

        return new TheoryData<UserExercise, int, string>
        {
            { new UserExercise { Id = 1, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, 1, TestServiceResponse.Success },
            { new UserExercise { Id = 0, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, 1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = null, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, 1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = -1, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, 1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = -1, Sets = 3, Timed = true, BodyWeight = false }, 1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = -1, Timed = true, BodyWeight = false }, 1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = ValidAdminUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, -1, TestServiceResponse.BadRequest },
            { new UserExercise { Id = 1, UserId = InvalidUserId, CurrentWeight = 100.0, PersonalRecord = 200.0, DurationInSeconds = 60, SortOrder = 1, Repetitions = 10, Sets = 3, Timed = true, BodyWeight = false }, -1, TestServiceResponse.BadRequest }
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}