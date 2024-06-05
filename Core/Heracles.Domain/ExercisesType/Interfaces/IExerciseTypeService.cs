using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Domain.ExercisesType.Interfaces;

public interface IExerciseTypeService
{
    Task<ServiceResponse<QueryResponseDto<ExerciseType>>> GetAsync(QueryRequestDto query);
    
    Task<ServiceResponse<ExerciseType>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(ExerciseType entity);
    Task<ServiceResponse<bool>> UpdateAsync(ExerciseType entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}

