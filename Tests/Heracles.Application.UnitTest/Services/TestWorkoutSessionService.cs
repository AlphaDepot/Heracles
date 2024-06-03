using Heracles.Application.Features.WorkoutSessions;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;

using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;
/// <summary>
/// This class contains unit tests for the WorkoutSessionService class.
/// </summary>
public class TestWorkoutSessionService : BaseUnitTest
{
    
    // services
    private readonly Mock<IAppLogger<WorkoutSessionService>> _logger;
    private readonly WorkoutSessionService _serviceWithAdminUser;
    private readonly WorkoutSessionService _serviceWithNormalUser;
    
    /// <summary>
    /// Constructor for TestWorkoutSessionService, initializes mock objects and test data.
    /// </summary>
    public TestWorkoutSessionService(ITestOutputHelper testConsole) : base(testConsole)
    {
        // test output helper
        _logger = new Mock<IAppLogger<WorkoutSessionService>>();
        
        // service  for admin and non admin user
        _serviceWithAdminUser = new WorkoutSessionService(_logger.Object, HttpContextAccessorWithAdminUser.Object, WorkoutSessionRepository.Object, UserExerciseRepository.Object, UserService.Object);
        _serviceWithNormalUser = new WorkoutSessionService(_logger.Object, HttpContextAccessorWithNormalUser.Object, WorkoutSessionRepository.Object, UserExerciseRepository.Object, UserService.Object);
        
    }

    /// <summary>
    /// Test case for GetAsync method with filter and sort parameters when user is  admin.
    /// </summary>
    ///  <param name="query">Query request</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsAdmin(QueryRequestDto query)
    {
        SearchTerm = "Test Workout Session 1";
        
        // Arrange
        var expected = WorkoutSessionFixture.Query(query, ValidAdminUserId, true);

        // Act
        var result = await _serviceWithAdminUser.GetAsync(query);

        TestConsole.WriteLine("Test");
        // Assert 
        ExpectedQueryResult.Success(result, expected);
        
    }
    
    /// <summary>
    ///  Test case for GetAsync method with filter and sort parameters when user is not admin.
    /// </summary>
    /// <param name="query"> Query request </param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsNotAdmin(QueryRequestDto query)
    {
        // Arrange
        var expected = WorkoutSessionFixture.Query(query, ValidUserId, false);

        // Act
        var result = await _serviceWithNormalUser.GetAsync(query);

        // Assert 
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    /// Test case for GetByIdAsync method.
    /// </summary>
    ///  <param name="id">Workout session id</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncFluentData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedWorkoutSession = WorkoutSessions.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _serviceWithAdminUser.GetByIdAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, expectedWorkoutSession!, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.ValidationFail(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    /// Test case for CreateAsync method.
    /// </summary>
    ///  <param name="userId">User id</param>
    ///  <param name="name">Workout session name</param>
    ///  <param name="dayOfWeek">Day of week</param>
    ///  <param name="sortOrder">Sort order</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string? userId, string name, DayOfWeek dayOfWeek, int sortOrder,
        string expected)
    {
        // Arrange
        var workoutSession = new WorkoutSession
        {
            UserId = userId ?? "",
            Name = name,
            DayOfWeek = dayOfWeek,
            SortOrder = sortOrder
        };

        // Act
        var result = await _serviceWithAdminUser.CreateAsync(workoutSession);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<WorkoutSessionService, WorkoutSession>(_logger, result, workoutSession.Id,
                result.Value);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<WorkoutSessionService, WorkoutSession>(_logger, result, result.Error.Errors!);
        
    }
    
    /// <summary>
    /// Test case for UpdateAsync method.
    /// </summary>
    ///  <param name="id">Workout session id</param>
    ///  <param name="userId">User id</param>
    ///  <param name="name">Workout session name</param>
    ///  <param name="dayOfWeek">Day of week</param>
    ///  <param name="sortOrder">Sort order</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string userId, string name, DayOfWeek dayOfWeek,
        int sortOrder, string expected)
    {
        // Arrange
        var workoutSession = WorkoutSessions.FirstOrDefault(q => q.Id == id)
                             ?? new WorkoutSession
                             {
                                 Id = id,
                                 UserId = userId,
                                 Name = name,
                                 DayOfWeek = dayOfWeek,
                                 SortOrder = sortOrder
                             };

        workoutSession.UserId = userId;
        workoutSession.Name = name;
        workoutSession.DayOfWeek = dayOfWeek;
        workoutSession.SortOrder = sortOrder;


        // Act
        var result = await _serviceWithAdminUser.UpdateAsync(workoutSession);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<WorkoutSessionService, WorkoutSession>(_logger, result, workoutSession.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<WorkoutSessionService, WorkoutSession>(_logger, result,result.Error.Errors!);

    }

    /// <summary>
    /// Test case for DeleteAsync method.
    /// </summary>
    ///  <param name="id">Workout session id</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncFluentData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedWorkoutSession = WorkoutSessions.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _serviceWithAdminUser.DeleteAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<WorkoutSessionService, WorkoutSession>(_logger, result, expectedWorkoutSession!.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.ValidationFail<WorkoutSessionService, WorkoutSession>(_logger, result, result.Error.Errors!);
        
    }

    /// <summary>
    /// Test case for AddUserExerciseAsync method.
    /// </summary>
    ///  <param name="userId">User id</param>
    ///  <param name="workoutSessionId">Workout session id</param>
    ///  <param name="userExerciseId">User exercise id</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(AddUserExerciseData))]
    public async Task AddUserExerciseAsync_ReturnsResponse(string userId, int workoutSessionId, int userExerciseId,
        string expected)
    {
        // Arrange
        var workoutSession = WorkoutSessions.FirstOrDefault(q => q.Id == workoutSessionId) ?? new WorkoutSession
        {
            Id = workoutSessionId,
            UserId = userId,
            Name = "Test Workout Session 1",
            DayOfWeek = DayOfWeek.Monday,
            SortOrder = 1
        };
        var userExercise = UserExercises.FirstOrDefault(q => q.Id == userExerciseId) ?? new UserExercise
        {
            Id = userExerciseId,
            UserId = userId,
            ExerciseTypeId = 1,
            CurrentWeight = 100,
            PersonalRecord = 100,
            SortOrder = 1
        };


        var workoutSessionExerciseDto = new WorkoutSessionExerciseDto
        {
            UserId = userId,
            WorkoutSessionId = workoutSessionId,
            UserExerciseId = userExerciseId
        };

        // Act
        var result = await _serviceWithAdminUser.AddUserExerciseAsync(workoutSessionExerciseDto);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<WorkoutSessionService, WorkoutSession>(_logger, result, workoutSessionId);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<WorkoutSessionService, WorkoutSession>(_logger, result,
                result.Error.Errors!);
    }
    /// <summary>
    /// Test case for RemoveUserExerciseAsync method.
    /// </summary>
    ///  <param name="userId">User id</param>
    ///  <param name="workoutSessionId">Workout session id</param>
    ///  <param name="userExerciseId">User exercise id</param>
    ///  <param name="expected">Expected response</param>
    [Theory]
    [MemberData(nameof(AddUserExerciseData))]
    public async Task RemoveUserExerciseAsync_ReturnsResponse(string userId, int workoutSessionId, int userExerciseId,
        string expected)
    {
        // Arrange
        var workoutSession = WorkoutSessions.FirstOrDefault(q => q.Id == workoutSessionId) ?? new WorkoutSession
        {
            Id = workoutSessionId,
            UserId = userId,
            Name = "Test Workout Session 1",
            DayOfWeek = DayOfWeek.Monday,
            SortOrder = 1
        };
        var userExercise = UserExercises.FirstOrDefault(q => q.Id == userExerciseId) ?? new UserExercise
        {
            Id = userExerciseId,
            UserId = userId,
            ExerciseTypeId = 1,
            CurrentWeight = 100,
            PersonalRecord = 100,
            SortOrder = 1
        };


        var workoutSessionExerciseDto = new WorkoutSessionExerciseDto
        {
            UserId = userId,
            WorkoutSessionId = workoutSessionId,
            UserExerciseId = userExerciseId
        };

        // Act
        var result = await _serviceWithAdminUser.RemoveUserExerciseAsync(workoutSessionExerciseDto);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<WorkoutSessionService, WorkoutSession>(_logger, result, workoutSessionId);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<WorkoutSessionService, WorkoutSession>(_logger, result,
                result.Error.Errors!);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///     Test case for CreateAsync method.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<string, string, DayOfWeek, int, string> CreateData()
    {
        
        return new TheoryData<string, string, DayOfWeek, int, string>
        {
            { ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success },
            { ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { null, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success },
            { ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success },
            { ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success },
            { ValidAdminUserId, "", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { ValidAdminUserId, "HE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { ValidAdminUserId, new string('a', 256), DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { ValidAdminUserId, null, DayOfWeek.Monday, 2, TestDomainResponse.BadRequest },
            { InvalidUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }
        };
        
    }

    /// <summary>
    ///     Test case for UpdateAsync method.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<int, string, string, DayOfWeek, int, string> UpdateData()
    {
        return new TheoryData<int, string, string, DayOfWeek, int, string>
        {
            {1, ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success},
            {1, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {0, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {-1, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, ValidAdminUserId, "", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, ValidAdminUserId, "HE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, ValidAdminUserId, new string('a', 256), DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, ValidAdminUserId, null, DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, null, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest},
            {1, InvalidUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest}
        };
    }
        

    /// <summary>
    /// Test case for AddUserExerciseAsync method.
    /// </summary>
    /// <returns> TheoryData </returns> 
    public static TheoryData<string, int, int, string> AddUserExerciseData()
    {
        return new TheoryData<string, int, int, string> {
            {ValidAdminUserId, 1, 1, TestDomainResponse.Success},
            {ValidAdminUserId, 1, 0, TestDomainResponse.BadRequest},
            {ValidAdminUserId, 0, 1, TestDomainResponse.BadRequest},
            {ValidAdminUserId, 0, 0, TestDomainResponse.BadRequest},
            {InvalidUserId, 1, 1, TestDomainResponse.BadRequest}
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}