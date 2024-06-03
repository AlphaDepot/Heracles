using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class ExerciseMuscleGroupRepository : GenericRepository<ExerciseMuscleGroup>, IExerciseMuscleGroupRepository
{
    public ExerciseMuscleGroupRepository(HeraclesDbContext dbContext) : base(dbContext)
    {
    }

    
    public async Task<QueryResponseDto<ExerciseMuscleGroup>> GetByExerciseIdAsync(QueryableEntityDto<ExerciseMuscleGroup> queryableDto)
    {
        var queryable = QueryBuilder(queryableDto);
        var result = await queryable.ToListAsync();
        
        var totalItems = await DbContext.Set<ExerciseMuscleGroup>().CountAsync();
        
        return new QueryResponseDto<ExerciseMuscleGroup>
        {
            Data = result,
            PageNumber = queryableDto.PageNumber,
            PageSize = queryableDto.PageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)queryableDto.PageSize),
            TotalItems = totalItems
        };
        
    }

    /// <summary>
    /// Checks if the combination of exercise ID, muscle group ID, and muscle function ID is unique.
    /// </summary>
    /// <param name="exerciseId">The ID of the exercise.</param>
    /// <param name="muscleGroupId">The ID of the muscle group.</param>
    /// <param name="muscleFunctionId">The ID of the muscle function.</param>
    /// <returns>Returns true if the combination is unique, otherwise false.</returns>
    public async Task<bool> IsUnique(int exerciseId, int muscleGroupId, int muscleFunctionId)
    {
        return !await DbContext.Set<ExerciseMuscleGroup>().AnyAsync(
            m =>
                m.ExerciseTypeId == exerciseId &&
                m.Muscle.Id == muscleGroupId &&
                m.Function.Id == muscleFunctionId
        );
    }
    
  
}