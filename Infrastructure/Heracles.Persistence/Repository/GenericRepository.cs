using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Abstractions.Extensions;
using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.Repository;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly HeraclesDbContext DbContext;
   

    public GenericRepository(HeraclesDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    
    /// <summary>
    /// Retrieves a list of entities asynchronously based on the provided filter, order by function, page size, and page number.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="query">The QueryableEntityDto object containing the filter, order by function, page size, and page number.</param>
    /// <returns>A task representing the asynchronous operation that returns a QueryResponseDto object containing the list of entities matching the provided criteria.</returns>
    public async Task<QueryResponseDto<T>> GetAsync(QueryableEntityDto<T> query)
    {
        var queryable = QueryBuilder(query);
        
        var result = await queryable.ToListAsync();
        var total = await DbContext.Set<T>().CountAsync();
        
        return new QueryResponseDto<T>
        {
            Data = result,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize), // convert total items to pages
            TotalItems = total
        };
    }


    /// <summary>
    /// Retrieves an entity asynchronously based on its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="includeProperties">The related entities to include in the query.</param>
    /// <returns>A task representing the asynchronous operation that returns the entity matching the provided ID.</returns>
    public async Task<T> GetByIdAsync(int id,params string[]? includeProperties)
    {
        IQueryable<T?> query = DbContext.Set<T>();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return  await query.FirstOrDefaultAsync(e => e.Id == id);
        

    }
    
    /// <summary>
    /// Retrieves an entity asynchronously based on its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>A task representing the asynchronous operation that returns the entity matching the provided ID.</returns>
    public async  Task<T> GetByIdAsync(int id)
    {
        var entity = await DbContext.Set<T>().FindAsync(id);
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
        DbContext.Set<T>().Add(entity);
        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates the provided entity asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation that returns the number of entities updated in the database.</returns>
    public async Task<int> UpdateAsync(T entity)
    {
        var existingEntity = await DbContext.Set<T>().FindAsync(entity.Id);
        if (existingEntity != null)
        {
            DbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            DbContext.Entry(entity).State = EntityState.Modified;
        }
        return await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity asynchronously based on its ID.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the asynchronous operation that returns the number of entities affected.</returns>
    public async Task<int> DeleteAsync(int id)
    {
        var entity = await DbContext.Set<T>().FindAsync(id);
        DbContext.Set<T>().Remove(entity);
        return await DbContext.SaveChangesAsync();
    }


    /// <summary>
    /// Checks if an entity with the specified id exists in the database.
    /// </summary>
    /// <param name="id">The id of the entity to check for existence.</param>
    /// <returns>True if an entity with the specified id does not exist, false otherwise.</returns>
    public async Task<bool> ItExist(int id)
    {
        var entity = await DbContext.Set<T>().FindAsync(id);
        
        return entity != null;
    }
    
    private IQueryable<T> QueryBuilder(QueryableEntityDto<T> query)
    {
        return DbContext.Set<T>().ApplyFilter(query)
            .SetSortingMode(query)
            .ApplyPaging(query);
    }
    
    
}