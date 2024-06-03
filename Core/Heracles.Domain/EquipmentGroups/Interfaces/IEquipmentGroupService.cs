using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Models;

namespace Heracles.Domain.EquipmentGroups.Interfaces;

public interface IEquipmentGroupService
{
    Task<DomainResponse<QueryResponseDto<EquipmentGroup>>> GetAsync(QueryRequestDto query);
    
    Task<DomainResponse<EquipmentGroup>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(EquipmentGroup entity);
    Task<DomainResponse<bool>> UpdateAsync(EquipmentGroup entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
    
    Task<DomainResponse<bool>> AddEquipmentAsync(AddRemoveEquipmentGroupDto entityDto);
    Task<DomainResponse<bool>> RemoveEquipmentAsync(AddRemoveEquipmentGroupDto entityDto);
}