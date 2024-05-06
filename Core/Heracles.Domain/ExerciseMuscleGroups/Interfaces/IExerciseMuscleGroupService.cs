using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Interfaces;

public interface IExerciseMuscleGroupService 
{
    Task<DomainResponse<QueryResponse<ExerciseMuscleGroup>>> GetAsync(QueryRequest? query );
    Task<DomainResponse<ExerciseMuscleGroup>> GetByIdAsync(int id);
    Task<DomainResponse<QueryResponse<ExerciseMuscleGroup>>> GetByExerciseIdAsync(int id, QueryRequest? query = null);
    Task<DomainResponse<int>> CreateAsync(CreateExerciseMuscleGroupDto entity);
    Task<DomainResponse<bool>> UpdateAsync(UpdateExerciseMuscleGroupDto entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}





