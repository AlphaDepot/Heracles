using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Domain.MuscleFunctions.Interfaces;

public interface IMuscleFunctionService
{
    Task<ServiceResponse<QueryResponseDto<MuscleFunction>>> GetAsync(QueryRequestDto query);
    Task<ServiceResponse<MuscleFunction>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(MuscleFunction entity);
    Task<ServiceResponse<bool>> UpdateAsync(MuscleFunction entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}