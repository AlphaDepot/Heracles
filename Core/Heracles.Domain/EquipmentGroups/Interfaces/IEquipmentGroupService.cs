using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Models;

namespace Heracles.Domain.EquipmentGroups.Interfaces;

public interface IEquipmentGroupService
{
    Task<ServiceResponse<QueryResponseDto<EquipmentGroup>>> GetAsync(QueryRequestDto query);
    
    Task<ServiceResponse<EquipmentGroup>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(EquipmentGroup entity);
    Task<ServiceResponse<bool>> UpdateAsync(EquipmentGroup entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
    
    Task<ServiceResponse<bool>> AddEquipmentAsync(AddRemoveEquipmentGroupDto entityDto);
    Task<ServiceResponse<bool>> RemoveEquipmentAsync(AddRemoveEquipmentGroupDto entityDto);
}