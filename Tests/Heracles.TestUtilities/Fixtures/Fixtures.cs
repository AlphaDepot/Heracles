using System.Linq.Expressions;
using System.Reflection;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;
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
    public static List<T> Query<T>(List<T> list, QueryRequestDto? query) where T : BaseEntity
    {
        // if query is null, return all exercise muscle groups
        if (query == null)
            return list;

        // Get the static method from the type
        var getFilterExpressionMethod = typeof(T).GetMethod("GetFilterExpression", BindingFlags.Static | BindingFlags.Public);
        var getSortExpressionMethod = typeof(T).GetMethod("GetSortExpression", BindingFlags.Static | BindingFlags.Public);

        // if search term is not null, get the filter expression
        var filter = getFilterExpressionMethod?.Invoke(null, [query.SearchTerm]) 
                         as Expression<Func<T, bool>> 
                     ?? throw new Exception("Filter expression is null.");

        // apply filter
        list = list.Where(filter.Compile()).ToList();

        // get the sort expression
        var sortExpressions = getSortExpressionMethod?.Invoke(null, null) 
                                  as Dictionary<string, Expression<Func<T, object>>> 
                              ?? throw new Exception("Sort expressions are null.");

        // create the queryable dto
        var sorter = new QueryHelper().Sorter(query, sortExpressions);

        // sort the list if sorter is not null
        if (sorter != null)
            list = sorter(list.AsQueryable()).ToList();

        //  // page size and page number
        list = list.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList();

        return list;
    }
    
    
    /// <summary>
    /// Executes a query on a list of entities with user information.
    /// </summary>
    /// <typeparam name="T">The type of entities in the list.</typeparam>
    /// <param name="list">The list of entities to query.</param>
    /// <param name="query">The query parameters.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="isAdmin">A flag indicating if the user is an admin.</param>
    /// <returns>The filtered and sorted list of entities based on the query parameters.</returns>
    public static List<T> QueryWithUser<T>(List<T> list, QueryRequestDto? query, string userId, bool isAdmin) where T : BaseEntity
    {
        // if query is null, return all exercise muscle groups
        if (query == null)
            return list;

        // Get the static method from the type or throw an exception
        var getFilterExpressionMethod = typeof(T).GetMethod("GetFilterExpression", BindingFlags.Static | BindingFlags.Public) ?? throw
            new Exception("Method GetFilterExpression not found in type " + typeof(T).Name);
        // Get the static method from the type or throw an exception
        var getSortExpressionMethod = typeof(T).GetMethod("GetSortExpression", BindingFlags.Static | BindingFlags.Public) ?? throw
            new Exception("Method GetSortExpression not found in type " + typeof(T).Name);

        // if search term is not null, get the filter expression
        if (getFilterExpressionMethod?.Invoke(null, new object[] { query.SearchTerm, userId, isAdmin }) is Expression<Func<T, bool>> filter)
            list = list.Where(filter.Compile()).ToList();

        // apply filter to the list
        if (getSortExpressionMethod?.Invoke(null, null) is not Dictionary<string, Expression<Func<T, object>>> sortExpressions)
            throw new Exception("Sort expressions are null.");

        // get the sort expression
        var sorter = new QueryHelper().Sorter(query, sortExpressions);
        // create the queryable dto
        if (sorter != null)
            list = sorter(list.AsQueryable()).ToList();
        // sort the list if sorter is not null
        list = list.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToList();

        return list;
    }
    
}