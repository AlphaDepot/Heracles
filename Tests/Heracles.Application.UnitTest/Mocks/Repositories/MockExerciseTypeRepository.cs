using System.Linq.Expressions;
using Heracles.Domain.Equipments.Models;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  This class is responsible for mocking the ExerciseTypeRepository.
/// </summary>
public  class MockExerciseTypeRepository : MockBaseRepository<ExerciseType, IExerciseTypeRepository>
{
    /// <summary>
    ///  Constructor for the MockExerciseTypeRepository.
    /// </summary>
    /// <param name="entities"> The list of ExerciseType entities.</param>
    public MockExerciseTypeRepository(List<ExerciseType> entities) : base(entities)
    {
        IsNameUniqueMock();
    }
    /// <summary>
    ///  Get the Mock for the ExerciseTypeRepository.
    /// </summary>
    /// <returns> The Mock for the ExerciseTypeRepository.</returns>
    public static Mock<IExerciseTypeRepository> Get()
    {
        return new MockExerciseTypeRepository(ExerciseTypeFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Mock for the IsNameUnique method of the ExerciseTypeRepository.
    ///  This method is used to check if the ExerciseType is unique.
    /// </summary>
    private void IsNameUniqueMock()
    {
        MockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => Entities.All(q => q.Name != name));
    }
    
    
}
