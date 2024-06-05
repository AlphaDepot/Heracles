using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Domain.MuscleGroups.Interfaces;

public interface IMuscleGroupService
{
    Task<ServiceResponse<QueryResponseDto<MuscleGroup>>> GetAsync(QueryRequestDto query);
    Task<ServiceResponse<MuscleGroup>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(MuscleGroup entity);
    Task<ServiceResponse<bool>> UpdateAsync(MuscleGroup entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}