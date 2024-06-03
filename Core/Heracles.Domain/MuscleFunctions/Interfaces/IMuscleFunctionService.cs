using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Domain.MuscleFunctions.Interfaces;

public interface IMuscleFunctionService
{
    Task<DomainResponse<QueryResponseDto<MuscleFunction>>> GetAsync(QueryRequestDto query);
    Task<DomainResponse<MuscleFunction>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(MuscleFunction entity);
    Task<DomainResponse<bool>> UpdateAsync(MuscleFunction entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}