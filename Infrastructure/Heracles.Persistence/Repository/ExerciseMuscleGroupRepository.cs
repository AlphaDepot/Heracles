using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.ExerciseMuscleGroups;
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


    /// <summary>
    /// Retrieves a list of ExerciseMuscleGroup entities by exercise ID.
    /// </summary>
    /// <param name="queryableDto">The query DTO containing the filter, sort, and pagination options.</param>
    /// <returns>The list of ExerciseMuscleGroup entities that match the provided exercise ID.</returns>
    public async Task<QueryResponse<ExerciseMuscleGroup>> GetByExerciseIdAsync(QuariableDto<ExerciseMuscleGroup> queryableDto)
    {
        IQueryable<ExerciseMuscleGroup> query = _dbContext.Set<ExerciseMuscleGroup>();
        // Apply filter
        if (queryableDto.Filter != null)
            query = query.Where(queryableDto.Filter);
        
        // Apply order by
        if (queryableDto.Sorter != null)
            query = queryableDto.Sorter(query);
        
        // Apply paging
        query = query.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize).Take(queryableDto.PageSize);
        
        var result = await query.ToListAsync();
        
        // get total pages
        var totalItems = await _dbContext.Set<ExerciseMuscleGroup>().CountAsync();
        
        // Return the list of entities
        //return await query.ToListAsync();
        
        return new QueryResponse<ExerciseMuscleGroup>
        {
            Data = result,
            PageNumber = queryableDto.PageNumber,
            PageSize = queryableDto.PageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)queryableDto.PageSize)
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
        return !await _dbContext.Set<ExerciseMuscleGroup>().AnyAsync(
            m =>
                m.ExerciseTypeId == exerciseId &&
                m.Muscle.Id == muscleGroupId &&
                m.Function.Id == muscleFunctionId
        );
    }
}