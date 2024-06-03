using System.Linq.Expressions;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  This class is responsible for mocking the UserExerciseHistoryRepository.
/// </summary>
public  class MockUserExerciseHistoryRepository : MockBaseRepository<UserExerciseHistory, IUserExerciseHistoryRepository>
{
    /// <summary>
    ///  Constructor for the MockUserExerciseHistoryRepository.
    /// </summary>
    /// <param name="entities"> The list of UserExerciseHistory entities.</param>
    public MockUserExerciseHistoryRepository(List<UserExerciseHistory> entities) : base(entities)
    {
        SetupUserExerciseHistorySpecificMocks();
    }
    /// <summary>
    ///  Get the Mock for the UserExerciseHistoryRepository.
    /// </summary>
    /// <returns> The Mock for the UserExerciseHistoryRepository.</returns>
    public static Mock<IUserExerciseHistoryRepository> Get()
    {
        return new MockUserExerciseHistoryRepository(UserExerciseHistoryFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Mock for the UserExerciseHistoryRepository. 
    /// </summary>
    private void SetupUserExerciseHistorySpecificMocks()
    {
        // Setup UserExerciseHistory specific mock methods here
        // None At this time
    }
    
    
}