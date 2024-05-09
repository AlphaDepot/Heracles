using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockExerciseMuscleGroupRepository
{
    public static Mock<IExerciseMuscleGroupRepository> Get()
    {
        var exerciseMuscleGroups = ExerciseMuscleGroupFixture.Get();
        
        var mockRepo = new Mock<IExerciseMuscleGroupRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<ExerciseMuscleGroup>>()))
            .ReturnsAsync((QuariableDto<ExerciseMuscleGroup> queryableDto) =>
            {
                var queryable = exerciseMuscleGroups.AsQueryable();
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
        
        mockRepo.Setup(r => r.GetByExerciseIdAsync(It.IsAny<QuariableDto<ExerciseMuscleGroup>>()))
            .ReturnsAsync((QuariableDto<ExerciseMuscleGroup> queryableDto) =>
            {
                var queryable = exerciseMuscleGroups.AsQueryable();
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
        
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!
            .ReturnsAsync((int id) => exerciseMuscleGroups.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<ExerciseMuscleGroup>()))
            .ReturnsAsync((ExerciseMuscleGroup exerciseMuscleGroup) =>
            {
                exerciseMuscleGroup.Id = exerciseMuscleGroups.Count + 1;
                exerciseMuscleGroups.Add(exerciseMuscleGroup);
                return exerciseMuscleGroup.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => exerciseMuscleGroups.Any(q => q.Id == id));
        
        
        mockRepo.Setup(r => r.IsUnique(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((int exerciseId, int muscleGroupId, int muscleFunctionId) =>
            {
                return !exerciseMuscleGroups.Any(q => q.ExerciseTypeId == exerciseId && q.Muscle.Id == muscleGroupId && q.Function.Id == muscleFunctionId);
            });
       
        
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<ExerciseMuscleGroup>()))
            .ReturnsAsync((ExerciseMuscleGroup exerciseMuscleGroup) =>
            {
                exerciseMuscleGroup.Id = exerciseMuscleGroups.Count + 1;
                exerciseMuscleGroups.Add(exerciseMuscleGroup);
                return exerciseMuscleGroup.Id;
            });
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<ExerciseMuscleGroup>()))
            .ReturnsAsync((ExerciseMuscleGroup exerciseMuscleGroup) =>
            {
                var index = exerciseMuscleGroups.FindIndex(q => q.Id == exerciseMuscleGroup.Id);
                exerciseMuscleGroups[index] = exerciseMuscleGroup;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = exerciseMuscleGroups.FindIndex(q => q.Id == id);
                exerciseMuscleGroups.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        return mockRepo;
        
    }
}