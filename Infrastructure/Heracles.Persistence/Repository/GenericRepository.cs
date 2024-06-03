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

    
    public async Task<T?> GetEntityByIdAsync(int id,params string[]? includeProperties)
    {
        IQueryable<T?> query = DbContext.Set<T>();
        query = includeProperties != null ? IncludeProperties(includeProperties) : query;
        return await query.FirstOrDefaultAsync(e => e != null && e.Id == id);
    }
    

    public virtual async  Task<T?> GetEntityByIdAsync(int id)
    {
        return await DbContext.Set<T>().FindAsync(id);
    }
    
    
    public virtual async  Task<int> CreateEntityAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        DbContext.Set<T>().Add(entity);
        return await DbContext.SaveChangesAsync();
    }

    public virtual  async Task<int> UpdateEntityAsync(T entity)
    {
        var existingEntity = await DbContext.Set<T>().FindAsync(entity.Id);
        if (existingEntity == null) return 0;
        entity.UpdatedAt = DateTime.UtcNow;
        DbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        return await DbContext.SaveChangesAsync();
    }
    
    public virtual async Task<int> DeleteEntityAsync(int id)
    {
        var entity = await DbContext.Set<T>().FindAsync(id);
        if (entity == null) return 0;
         
        DbContext.Set<T>().Remove(entity);
        return await DbContext.SaveChangesAsync(); 
    }
    
    public virtual async Task<bool> ItExist(int id)
    {
        return await DbContext.ExerciseTypes.AnyAsync(x => x.Id == id);
    }
    
    
    private IQueryable<T> QueryBuilder(QueryableEntityDto<T> query)
    {
        return DbContext.Set<T>().ApplyFilter(query)
            .SetSortingMode(query)
            .ApplyPaging(query);
    }
    
    private IQueryable<T> IncludeProperties( params string[] includeProperties)
    {
        IQueryable<T> query = DbContext.Set<T>();
        query = includeProperties.Aggregate(query, (current, includeProperty) => 
            current.Include(includeProperty));
        return query;
    }
    
}