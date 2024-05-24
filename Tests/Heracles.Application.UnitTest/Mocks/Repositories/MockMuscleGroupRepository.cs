using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  This class is responsible for mocking the MuscleGroupRepository.
/// </summary>
public  class MockMuscleGroupRepository : MockBaseRepository<MuscleGroup, IMuscleGroupRepository>
{
    
    /// <summary>
    ///  Constructor for the MockMuscleGroupRepository.
    /// </summary>
    /// <param name="entities"> The list of MuscleGroup entities.</param>
    public MockMuscleGroupRepository(List<MuscleGroup> entities) : base(entities)
    {
        IsNameUniqueMock();
    }
    /// <summary>
    ///  Get the Mock for the MuscleGroupRepository.
    /// </summary>
    /// <returns> The Mock for the MuscleGroupRepository.</returns>
    public static Mock<IMuscleGroupRepository> Get()
    {
        return new MockMuscleGroupRepository(MuscleGroupFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Mock for the IsNameUnique method of the MuscleGroupRepository.
    ///  This method is used to check if the MuscleGroup is unique.
    /// </summary>
    private void IsNameUniqueMock()
    {
        // Setup MuscleGroup specific mock methods here
        MockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => Entities.All(q => q.Name != name));
    }
    
    
}