using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Domain.Abstractions.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> queryable,   QueryableEntityDto<T> queryableEntityDto ) where T : BaseEntity
    {
        return queryable.Where(queryableEntityDto.Filter ?? (e => true));
    }

    public static IQueryable<T> SetSortingMode<T>(this IQueryable<T> queryable,  QueryableEntityDto<T> queryableEntityDto) where T : BaseEntity
    {
        return queryableEntityDto.Sorter != null ? queryableEntityDto.Sorter(queryable) : queryable.OrderBy(e => e.UpdatedAt);
    }

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, QueryableEntityDto<T> queryableEntityDto) where T : BaseEntity
    {
        return queryable.Skip((queryableEntityDto.PageNumber  - 1) * queryableEntityDto.PageSize).Take(queryableEntityDto.PageSize);
    }
}