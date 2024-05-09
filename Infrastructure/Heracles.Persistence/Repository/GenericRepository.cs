using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly HeraclesDbContext _dbContext;
   

    public GenericRepository(HeraclesDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    /// <summary>
    /// Retrieves a list of entities asynchronously based on the provided filter, order by function, page size, and page number.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="query">The QuariableDto object containing the filter, order by function, page size, and page number.</param>
    /// <returns>A task representing the asynchronous operation that returns a QueryResponse object containing the list of entities matching the provided criteria.</returns>
    public async Task<QueryResponse<T>> GetAsync(QuariableDto<T> query)
    {
        IQueryable<T> queryable = _dbContext.Set<T>();
        
        // Apply filter
        if (query.Filter != null)
            queryable = queryable.Where(query.Filter);
        
        // Apply order by
        if (query.Sorter != null)
            queryable = query.Sorter(queryable);
        
        // Apply paging
        queryable = queryable.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
        
        var result = await queryable.ToListAsync();
        
        // get total pages
        var totalItems = await _dbContext.Set<T>().CountAsync();
        
        return new QueryResponse<T>
        {
            Data = result,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    

    /// <summary>
    /// Retrieves an entity asynchronously based on its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>A task representing the asynchronous operation that returns the entity matching the provided ID.</returns>
    public async Task<T> GetByIdAsync(int id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);
        return entity;
    }

    /// <summary>
    /// Creates a new entity asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="entity">The entity to create.</param>
    /// <returns>A task representing the asynchronous operation that returns the ID of the created entity.</returns>
    public async Task<int> CreateAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        _dbContext.Set<T>().Add(entity);
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the provided entity asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation that returns the number of entities updated in the database.</returns>
    public async Task<int> UpdateAsync(T entity)
    {
        var existingEntity = await _dbContext.Set<T>().FindAsync(entity.Id);
        if (existingEntity != null)
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity asynchronously based on its ID.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the asynchronous operation that returns the number of entities affected.</returns>
    public async Task<int> DeleteAsync(int id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);
        _dbContext.Set<T>().Remove(entity);
        return await _dbContext.SaveChangesAsync();
    }


    /// <summary>
    /// Checks if an entity with the specified id exists in the database.
    /// </summary>
    /// <param name="id">The id of the entity to check for existence.</param>
    /// <returns>True if an entity with the specified id does not exist, false otherwise.</returns>
    public async Task<bool> ItExist(int id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);
        
        return entity != null;
    }
}