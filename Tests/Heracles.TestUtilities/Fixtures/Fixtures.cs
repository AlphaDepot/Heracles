using System.Linq.Expressions;
using System.Reflection;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Users.Models;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///   Fixture  for all entities based on BaseEntity for unit tests
/// </summary>
public   static class Fixtures
{
    /// <summary>
    /// Executes a query on a list of entities.
    /// </summary>
    /// <typeparam name="T">The type of entities in the list.</typeparam>
    /// <param name="list">The list of entities to query.</param>
    /// <param name="query">The query parameters.</param>
    /// <returns>The filtered and sorted list of entities based on the query parameters.</returns>
    public static List<T> Query<T>(List<T> list, QueryRequest? query) where T : BaseEntity
    {
        // if query is null, return all exercise muscle groups
        if (query == null)
            return list;

        // Get the static method from the type
        var getFilterExpressionMethod = typeof(T).GetMethod("GetFilterExpression", BindingFlags.Static | BindingFlags.Public);
        var getSortExpressionMethod = typeof(T).GetMethod("GetSortExpression", BindingFlags.Static | BindingFlags.Public);

        // if search term is not null, get the filter expression
        var filter = (Expression<Func<T, bool>>)getFilterExpressionMethod?.Invoke(null, new object[] { query.SearchTerm });

        // apply filter
        list = list.Where(filter.Compile()).ToList();

        // get the sort expression
        var sortExpressions = (Dictionary<string, Expression<Func<T, object>>>)getSortExpressionMethod?.Invoke(null, null);

        // create the queryable dto
        var sorter = new QueryHelper().Sorter(query, sortExpressions);

        // sort the list if sorter is not null
        if (sorter != null)
            list = sorter(list.AsQueryable()).ToList();

        //  // page size and page number
        list = list.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList();

        return list;
    }
    
    
   public static List<T> QueryWithUser<T>(List<T> list, QueryRequest? query, string userId, bool isAdmin) where T : BaseEntity
{
    if (query == null)
        return list;

    var getFilterExpressionMethod = typeof(T).GetMethod("GetFilterExpression", BindingFlags.Static | BindingFlags.Public);
    var getSortExpressionMethod = typeof(T).GetMethod("GetSortExpression", BindingFlags.Static | BindingFlags.Public);

    if (getFilterExpressionMethod == null || getSortExpressionMethod == null)
        throw new Exception("Methods GetFilterExpression or GetSortExpression not found in type " + typeof(T).Name);
    
    var filter = (Expression<Func<T, bool>>)getFilterExpressionMethod?.Invoke(null, new object[] { query.SearchTerm, userId, isAdmin});
    
    if (filter != null)
        list = list.Where(filter.Compile()).ToList();

    var sortExpressions = (Dictionary<string, Expression<Func<T, object>>>)getSortExpressionMethod?.Invoke(null, null);
    if (sortExpressions == null)
        throw new Exception("Sort expressions are null.");

    var sorter = new QueryHelper().Sorter(query, sortExpressions);
    
    if (sorter != null)
        list = sorter(list.AsQueryable()).ToList();

    list = list.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList();

    return list;
}
    
}