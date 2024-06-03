using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Domain.MuscleGroups.Interfaces;

public interface IMuscleGroupService
{
    Task<DomainResponse<QueryResponseDto<MuscleGroup>>> GetAsync(QueryRequestDto query);
    Task<DomainResponse<MuscleGroup>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(MuscleGroup entity);
    Task<DomainResponse<bool>> UpdateAsync(MuscleGroup entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}