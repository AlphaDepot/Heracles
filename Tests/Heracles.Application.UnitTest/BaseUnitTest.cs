using Heracles.Application.UnitTest.Mocks.Repositories;
using Heracles.Application.UnitTest.Mocks.Services;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Heracles.TestUtilities.TestData;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit.Abstractions;


namespace Heracles.Application.UnitTest;
/// <summary>
///  Base class for all unit tests.
/// </summary>
public class BaseUnitTest
{
    // Search term 
    public static string SearchTerm { get; set; } = "test";
    
    // Constants
    protected const string NotADuplicateName = "NOT A DUPLICATE NAME";
    protected static readonly string ValidAdminUserId = UserExerciseSeedData.Users().First()!.UserId;
    protected static readonly string ValidUserId = UserExerciseSeedData.Users().Last()!.UserId;
    protected static readonly string SecondValidUserId = UserExerciseSeedData.Users().ElementAt(1)!.UserId;
    protected static readonly string InvalidUserId = "i"; // invalid user id for testing must be less than 2 characters
    
    // seed data for the tests
    protected readonly List<Equipment> Equipments = EquipmentFixture.Get();
    protected readonly List<EquipmentGroup> EquipmentGroups = EquipmentGroupFixture.Get();
    protected readonly List<ExerciseMuscleGroup> ExerciseMuscleGroups = ExerciseMuscleGroupFixture.Get();
    protected readonly List<ExerciseType> ExerciseTypes =ExerciseTypeFixture.Get();
    protected readonly List<MuscleFunction> MuscleFunctions = MuscleFunctionFixture.Get();
    protected readonly List<MuscleGroup> MuscleGroups = MuscleGroupFixture.Get();
    protected readonly List<UserExerciseHistory> UserExerciseHistory = UserExerciseHistoryFixture.Get();
    protected readonly List<UserExercise> UserExercises = UserExerciseFixture.Get();
    protected readonly List<WorkoutSession> WorkoutSessions = WorkoutSessionFixture.Get();
    protected readonly List<User> Users = UserFixture.Get()!;
    
    // Mocks
    // mock repositories
    protected readonly Mock<IEquipmentRepository> EquipmentRepository = MockEquipmentRepository.Get();
    protected readonly Mock<IEquipmentGroupRepository> EquipmentGroupRepository = MockEquipmentGroupRepository.Get();
    protected readonly Mock<IExerciseMuscleGroupRepository> ExerciseMuscleGroupRepository = MockExerciseMuscleGroupRepository.Get();
    protected readonly Mock<IExerciseTypeRepository> ExerciseTypeRepository = MockExerciseTypeRepository.Get();
    protected readonly Mock<IMuscleFunctionRepository> MuscleFunctionRepository = MockMuscleFunctionRepository.Get();
    protected readonly Mock<IMuscleGroupRepository> MuscleGroupRepository = MockMuscleGroupRepository.Get();
    protected readonly Mock<IUserExerciseHistoryRepository> UserExerciseHistoryRepository = MockUserExerciseHistoryRepository.Get();
    protected readonly Mock<IUserRepository> UserRepository =  MockUserRepository.Get();
    protected readonly Mock<IUserExerciseRepository> UserExerciseRepository = MockUserExerciseRepository.Get();
    protected readonly Mock<IWorkoutSessionRepository> WorkoutSessionRepository = MockWorkoutSessionRepository.Get();
    // mock services
    protected readonly Mock<IUserService> UserService = MockUserService.Get();
    
    // set up the http context accessor with admin and non admin user
    protected readonly Mock<IHttpContextAccessor> HttpContextAccessorWithAdminUser = MockHttpContextAccessor.GetAdmin();
    protected readonly Mock<IHttpContextAccessor> HttpContextAccessorWithNormalUser = MockHttpContextAccessor.Get();
    
    // Set up the test output helper
    protected readonly ITestOutputHelper TestConsole;

    public BaseUnitTest(ITestOutputHelper testConsole)
    {
        TestConsole = testConsole;
    }

    
    /// <summary>
    ///  Test case for UpdateUserExerciseAsync method.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<QueryRequest> QueryData()
    {
        var data = new TheoryData<QueryRequest>
        {
            new QueryRequest(),
            new QueryRequest { SearchTerm = SearchTerm },
            new QueryRequest { SortColumn = "id", SortOrder = "asc" },
            new QueryRequest { SortColumn = "id", SortOrder = "desc" },
            new QueryRequest { SortColumn = "created", SortOrder = "asc" },
            new QueryRequest { SortColumn = "created", SortOrder = "desc" },
            new QueryRequest { SortColumn = "updated", SortOrder = "asc" },
            new QueryRequest { SortColumn = "updated", SortOrder = "desc" },
            new QueryRequest { SortColumn = "name", SortOrder = "desc" },
            new QueryRequest { SortColumn = "name", SortOrder = "asc" },
            new QueryRequest { SortColumn = "dayofweek", SortOrder = "asc" },
            new QueryRequest { SortColumn = "dayofweek", SortOrder = "desc" },
            new QueryRequest { SortColumn = "sortorder", SortOrder = "asc" },
            new QueryRequest { SortColumn = "sortorder", SortOrder = "desc" },
            new QueryRequest { PageSize = 1, PageNumber = 3 }
        };
        return data;
    }


    /// <summary>
    ///  Test case for GetByIdAsync method.
    ///  This version is used when the data is validated using FluentValidation.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<int, string> GetByIdAsyncFluentData()
    {
        return new TheoryData<int, string>
        {
            { 0, TestDomainResponse.BadRequest }, // invalid id
            { -1, TestDomainResponse.BadRequest }, // invalid id
            { 1000000, TestDomainResponse.BadRequest }, // out of bound id
            { 1, TestDomainResponse.Success } // valid id
        };
    }

    /// <summary>
    ///  Test for the GetByIdAsync method.
    ///  This version is used when the data is validated using the ModelState.
    /// </summary>
    /// <returns> TheoryData </returns>   
    public static TheoryData<int, string> GetByIdAsyncData()
    {
        return new TheoryData<int, string>
        {
            { 0, TestDomainResponse.BadRequest }, // invalid id
            { -1, TestDomainResponse.BadRequest }, // invalid id
            { 1000000, TestDomainResponse.NotFound }, // out of bound id
            { 1, TestDomainResponse.Success } // valid id
        };
    }

    /// <summary>
    ///  Test case for DeleteAsync method.
    /// This version is used when the data is validated using FluentValidation.
    /// </summary>
    /// <returns> TheoryData </returns>   
    public static TheoryData<int, string> DeleteAsyncFluentData()
    {
        return new TheoryData<int, string>
        {
            { 0, TestDomainResponse.BadRequest }, // invalid id
            { -1, TestDomainResponse.BadRequest }, // invalid id
            { 1000000, TestDomainResponse.BadRequest }, // out of bound id
            { 1, TestDomainResponse.Success } // valid id
        };
    }

    /// <summary>
    ///  Test for the DeleteAsync method.
    ///  This version is used when the data is validated using the ModelState.
    /// </summary>
    /// <returns> TheoryData </returns>  
    public static TheoryData<int, string> DeleteAsyncData()
    {
        return new TheoryData<int, string>
        {
            { 0, TestDomainResponse.BadRequest }, // invalid id
            { -1, TestDomainResponse.BadRequest }, // invalid id
            { 1000000, TestDomainResponse.NotFound }, // out of bound id
            { 1, TestDomainResponse.Success } // valid id
        };
    }
   
}

