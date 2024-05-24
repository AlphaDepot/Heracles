
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  MockWorkoutSessionRepository is a mock repository for the WorkoutSession entity
/// </summary>
public  class MockWorkoutSessionRepository : MockBaseRepository<WorkoutSession, IWorkoutSessionRepository>
{
    
    /// <summary>
    ///  Create a new instance of the MockWorkoutSessionRepository  
    /// </summary>
    /// <param name="entities"> The list of WorkoutSession entities to use for the mock repository </param>
    public MockWorkoutSessionRepository(List<WorkoutSession> entities) : base(entities)
    {
        IsUniqueMock();
    }
    
    /// <summary>
    ///   Get a new instance of the MockWorkoutSessionRepository
    /// </summary>
    /// <returns> A new instance of the MockWorkoutSessionRepository </returns>
    public new static Mock<IWorkoutSessionRepository> Get()
    {
        return new MockWorkoutSessionRepository(WorkoutSessionFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Set up the IsUnique mock method for the WorkoutSessionRepository
    ///  This method is used to check if a WorkoutSession name is unique.
    /// </summary>
    private void IsUniqueMock()
    {
        // Setup WorkoutSession specific mock methods here
        MockRepo.Setup(r => r.IsUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => Entities.All(q => q.Name != name));
    }
    
    
}