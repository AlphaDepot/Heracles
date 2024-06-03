using System.Linq.Expressions;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Domain.Abstractions.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<QueryResponseDto<T>> GetAsync(QueryableEntityDto<T> query);
    Task<T> GetByIdAsync(int id);
    Task<T> GetByIdAsync(int id, params string[]? includeProperties);
    Task<int> CreateAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
    Task<bool> ItExist(int id);
}


