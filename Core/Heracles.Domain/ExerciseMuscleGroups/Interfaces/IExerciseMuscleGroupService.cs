using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Interfaces;

public interface IExerciseMuscleGroupService 
{
    Task<DomainResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetAsync(QueryRequestDto? query );
    Task<DomainResponse<ExerciseMuscleGroup>> GetByIdAsync(int id);
    Task<DomainResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetByExerciseIdAsync(int id, QueryRequestDto? query = null);
    Task<DomainResponse<int>> CreateAsync(CreateExerciseMuscleGroupDto entity);
    Task<DomainResponse<bool>> UpdateAsync(UpdateExerciseMuscleGroupDto entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}





