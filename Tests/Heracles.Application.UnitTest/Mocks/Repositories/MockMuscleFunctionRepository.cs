using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  This class is responsible for mocking the MuscleFunctionRepository.
/// </summary>
public class MockMuscleFunctionRepository : MockBaseRepository<MuscleFunction, IMuscleFunctionRepository>
{
    /// <summary>
    ///  Constructor for the MockMuscleFunctionRepository.
    /// </summary>
    /// <param name="entities"> The list of MuscleFunction entities.</param>
    public MockMuscleFunctionRepository(List<MuscleFunction> entities) : base(entities)
    {
        IsNameUniqueMock();
    }
    /// <summary>
    ///     Get the Mock for the MuscleFunctionRepository.
    /// </summary>
    /// <returns> The Mock for the MuscleFunctionRepository.</returns>
    public static Mock<IMuscleFunctionRepository> Get()
    {
        return new MockMuscleFunctionRepository(MuscleFunctionFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Mock for the IsNameUnique method of the MuscleFunctionRepository.
    ///  This method is used to check if the MuscleFunction is unique.
    /// </summary>
    private void IsNameUniqueMock()
    {
        // Setup MuscleFunction specific mock methods here
        MockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => Entities.All(q => q.Name != name));
    }
    
}
