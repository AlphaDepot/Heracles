using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Domain.Equipments.Interfaces;

public interface IEquipmentService
{
    Task<ServiceResponse<QueryResponseDto<Equipment>>> GetAsync(QueryRequestDto query);
    
    Task<ServiceResponse<Equipment>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(Equipment entity);
    Task<ServiceResponse<bool>> UpdateAsync(Equipment entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
    
}