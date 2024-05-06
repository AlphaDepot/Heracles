using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Domain.Equipments.Interfaces;

public interface IEquipmentService
{
    Task<DomainResponse<QueryResponse<Equipment>>> GetAsync(QueryRequest query);
    
    Task<DomainResponse<Equipment>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(Equipment entity);
    Task<DomainResponse<bool>> UpdateAsync(Equipment entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
    
}