using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Interfaces;

public interface IExerciseMuscleGroupRepository : IGenericRepository<ExerciseMuscleGroup>
{


    Task<QueryResponse<ExerciseMuscleGroup>> GetByExerciseIdAsync(QuariableDto<ExerciseMuscleGroup> queryableDto);
    
    Task<bool> IsUnique(int exerciseId, int muscleGroupId, int muscleFunctionId);

    
}