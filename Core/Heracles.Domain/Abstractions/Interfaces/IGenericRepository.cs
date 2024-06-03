using System.Linq.Expressions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Domain.Abstractions.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<QueryResponseDto<T>> GetAsync(QueryableEntityDto<T> query);
    Task<T?> GetEntityByIdAsync(int id);
    Task<T?> GetEntityByIdAsync(int id, params string[]? includeProperties);
    Task<int> CreateEntityAsync(T entity);
    Task<int> UpdateEntityAsync(T entity);
    Task<int> DeleteEntityAsync(int id);
    Task<bool> ItExist(int id);
}


