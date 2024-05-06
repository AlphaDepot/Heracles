using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Domain.MuscleFunctions.Interfaces;

public interface IMuscleFunctionService
{
    Task<DomainResponse<QueryResponse<MuscleFunction>>> GetAsync(QueryRequest query);
    Task<DomainResponse<MuscleFunction>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(MuscleFunction entity);
    Task<DomainResponse<bool>> UpdateAsync(MuscleFunction entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}