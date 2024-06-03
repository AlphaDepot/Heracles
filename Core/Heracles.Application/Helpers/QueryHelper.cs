using System.Linq.Expressions;
using Heracles.Domain.Abstractions.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Heracles.Application.Helpers;

/// <summary>
/// Helper class for creating query objects and sort expressions.
/// </summary>
public class QueryHelper
{
    /// <summary>
    /// Creates a QueriableDto object with the specified query, sort expressions, and filter expression.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="query">The query request.</param>
    /// <param name="sortExpressions">The sort expressions.</param>
    /// <param name="filterExpression">The filter expression.</param>
    /// <returns>A QueriableDto object.</returns>
    public QueryableEntityDto<T> CreateQueriable<T>(QueryRequestDto? query,
        Dictionary<string, Expression<Func<T, object>>> sortExpressions, Expression<Func<T, bool>> filterExpression)
    {
        // Filter the exercise muscle groups
        Expression<Func<T, bool>> filter = filterExpression;
         
        // Sort the exercise muscle groups
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = Sorter(query, sortExpressions);
        
        
        // Return the quariable dto
        return new QueryableEntityDto<T>
        {
            Filter = filter,
            Sorter = orderBy,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber
        };
    }

    /// <summary>
    /// Determines the sort order based on the specified query and sort expressions.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="query">The query request.</param>
    /// <param name="sortExpressions">The sort expressions.</param>
    /// <returns>A function that sorts the data in the specified order.</returns>
    public Func<IQueryable<T>, IOrderedQueryable<T>>? Sorter<T>(QueryRequestDto? query,
        Dictionary<string, Expression<Func<T, object>>> sortExpressions)
    {
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null;
        
        // If the query is null, return the orderBy
        if (query == null)
            return orderBy;

        if (!string.IsNullOrEmpty(query.SortColumn) && !string.IsNullOrEmpty(query.SortOrder) && sortExpressions.ContainsKey(query.SortColumn.ToLower()))
        {
            var sortExpression = sortExpressions[query.SortColumn.ToLower()];
            if (query.SortOrder.ToLower() == "desc")
                orderBy = q => q.OrderByDescending(sortExpression);
            else
                orderBy = q => q.OrderBy(sortExpression);
        }
        
        return orderBy;
    }
}