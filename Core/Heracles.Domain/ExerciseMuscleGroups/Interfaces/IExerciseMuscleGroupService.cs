using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Models;

namespace Heracles.Domain.ExerciseMuscleGroups.Interfaces;

public interface IExerciseMuscleGroupService 
{
    Task<ServiceResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetAsync(QueryRequestDto? query );
    Task<ServiceResponse<ExerciseMuscleGroup>> GetByIdAsync(int id);
    Task<ServiceResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetByExerciseIdAsync(int id, QueryRequestDto? query = null);
    Task<ServiceResponse<int>> CreateAsync(CreateExerciseMuscleGroupDto entity);
    Task<ServiceResponse<bool>> UpdateAsync(UpdateExerciseMuscleGroupDto entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}





