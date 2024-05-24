using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

/// <summary>
///   This class is responsible for mocking the ExerciseMuscleGroupRepository.
/// </summary>
public class MockExerciseMuscleGroupRepository : MockBaseRepository<ExerciseMuscleGroup, IExerciseMuscleGroupRepository>
{
    /// <summary>
    ///  Constructor for the MockExerciseMuscleGroupRepository.
    /// </summary>
    /// <param name="entities"> The list of ExerciseMuscleGroup entities.</param>
    public MockExerciseMuscleGroupRepository(List<ExerciseMuscleGroup> entities) : base(entities)
    {
        IsUniqueMock();
        GetByExerciseIdMock();
    }

    /// <summary>
    ///  Get the Mock for the ExerciseMuscleGroupRepository.
    /// </summary>
    /// <returns> The Mock for the ExerciseMuscleGroupRepository.</returns>
    public new static Mock<IExerciseMuscleGroupRepository> Get()
    {
        return new MockExerciseMuscleGroupRepository(ExerciseMuscleGroupFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///   Mock for the IsUnique method of the ExerciseMuscleGroupRepository.
    ///  This method is used to check if the ExerciseMuscleGroup is unique.
    /// </summary>
    private void IsUniqueMock()
    {
        MockRepo.Setup(r => r.IsUnique(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((int exerciseId, int muscleGroupId, int muscleFunctionId) =>
            {
                return !Entities.Any(q =>
                    q.ExerciseTypeId == exerciseId && q.Muscle.Id == muscleGroupId &&
                    q.Function.Id == muscleFunctionId);
            });
    }

    /// <summary>
    ///  Mock for the GetByExerciseId method of the ExerciseMuscleGroupRepository.
    ///  This method is used to get all the MuscleGroups for a given ExerciseId.
    /// </summary>
    private void GetByExerciseIdMock()
    {
        MockRepo.Setup(r => r.GetByExerciseIdAsync(It.IsAny<QuariableDto<ExerciseMuscleGroup>>()))
            .ReturnsAsync((QuariableDto<ExerciseMuscleGroup> queryableDto) =>
            {
                var queryable = Entities.AsQueryable();
                if (queryableDto.Filter != null)
                {
                    queryable = queryable.Where(queryableDto.Filter);
                }
                if (queryableDto.Sorter != null)
                {
                    queryable = queryableDto.Sorter(queryable);
                }
                var result =  queryable.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize).Take(queryableDto.PageSize).ToList();

                return new QueryResponse<ExerciseMuscleGroup>()
                {
                    Data =  result,
                    TotalPages = result.Count(),
                    PageSize = queryableDto.PageSize,
                    PageNumber = queryableDto.PageNumber
                };

            });
    }

    
}