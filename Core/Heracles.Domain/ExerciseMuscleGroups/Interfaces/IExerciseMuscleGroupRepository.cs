using System.Linq.Expressions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Interfaces;

public interface IExerciseMuscleGroupRepository : IGenericRepository<ExerciseMuscleGroup>
{


    Task<QueryResponseDto<ExerciseMuscleGroup>> GetByExerciseIdAsync(QueryableEntityDto<ExerciseMuscleGroup> queryableDto);
    
    Task<bool> IsUnique(int exerciseId, int muscleGroupId, int muscleFunctionId);

    
}