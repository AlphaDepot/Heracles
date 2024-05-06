using Heracles.Application.Features.WorkoutSessions;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
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
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsAdmin(QueryRequest query)
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
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedData_WhenUserIsNotAdmin(QueryRequest query)
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
            UserId = userId,
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

    

    
    public static IEnumerable<object?[]> CreateData()
    {
        

        // userId, name, dayOfWeek, sortOrder, expected
        yield return new object?[] { ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success };
        yield return new object?[]
            { ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // duplicate

        yield return new object?[]
            { null, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid userId
        yield return
            new object?[] { ValidAdminUserId, "NOT DUPLICATE", -1, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return
            new object?[] { ValidAdminUserId, "NOT DUPLICATE", 8, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return new object?[]
            { ValidAdminUserId, "NOT DUPLICATE", null, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return new object?[] { ValidAdminUserId, "", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[] { ValidAdminUserId, "HE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[]
            { ValidAdminUserId, new string('a', 256), DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[] { ValidAdminUserId, null, DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name

        
         yield return new object?[] { InvalidUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid userId
         yield return new object?[] { null, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid userId
    }
    
    /// <summary>
    ///     Test case for UpdateAsync method.
    /// </summary>
    /// <returns> Task </returns>
    public static IEnumerable<object?[]> UpdateData()
    {
        
        // id, userId, name, dayOfWeek, sortOrder, expected
        yield return new object?[] { 1, ValidAdminUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.Success };

        yield return new object?[]
            { 1, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // duplicate
        yield return new object?[]
            { 0, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid id
        yield return new object?[]
            { -1, ValidAdminUserId, "Test Workout Session 1", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid id


        yield return new object?[]
            { 1, ValidAdminUserId, "NOT DUPLICATE", -1, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return new object?[]
            { 1, ValidAdminUserId, "NOT DUPLICATE", 8, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return new object?[]
            { 1, ValidAdminUserId, "NOT DUPLICATE", null, 2, TestDomainResponse.BadRequest }; // invalid day of week
        yield return new object?[] { 1, ValidAdminUserId, "", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[]
            { 1, ValidAdminUserId, "HE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[]
            { 1, ValidAdminUserId, new string('a', 256), DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        yield return new object?[]
            { 1, ValidAdminUserId, null, DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid name
        
        yield return new object?[] { 1, null, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid userId
        yield return new object?[] {1, InvalidUserId, "NOT DUPLICATE", DayOfWeek.Monday, 2, TestDomainResponse.BadRequest }; // invalid userId
    }

    /// <summary>
    ///   Test case for AddUserExerciseAsync method.
    /// </summary>
    /// <returns> Task </returns>
    public static IEnumerable<object[]> AddUserExerciseData()
    {
        yield return new object[] { ValidAdminUserId, 1, 1, TestDomainResponse.Success };
        yield return new object[] { ValidAdminUserId, 1, 0, TestDomainResponse.BadRequest };
        yield return new object[] { ValidAdminUserId, 0, 1, TestDomainResponse.BadRequest };
        yield return new object[] { ValidAdminUserId, 0, 0, TestDomainResponse.BadRequest };
        yield return new object[] {InvalidUserId,  1, 1, TestDomainResponse.BadRequest };
    }

    
    
}