using System.Linq.Expressions;

namespace Heracles.Domain.Abstractions.DTOs;

public class QueryableEntityDto<T>
{
    public Expression<Func<T, bool>>? Filter { get; set; }
    public Func<IQueryable<T>, IOrderedQueryable<T>>? Sorter { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}