using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Domain.ExercisesType.Interfaces;

public interface IExerciseTypeService
{
    Task<DomainResponse<QueryResponseDto<ExerciseType>>> GetAsync(QueryRequestDto query);
    
    Task<DomainResponse<ExerciseType>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(ExerciseType entity);
    Task<DomainResponse<bool>> UpdateAsync(ExerciseType entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}

