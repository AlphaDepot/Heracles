using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Requests;

namespace Application.Common.Utilities;

/// <inheritdoc cref="IQueryableBuilder{T}" />
public abstract class QueryableBuilderBase<T> : IQueryableBuilder<T> where T : IEntity
{
	/// <inheritdoc />
	public virtual IQueryable<T> ApplyFilter(IQueryable<T> queryable, QueryRequest query)
	{
		// Default implementation (no filter)
		return queryable;
	}

	/// <inheritdoc />
	public virtual IQueryable<T> ApplySorting(IQueryable<T> queryable, QueryRequest query)
	{
		// Default sorting by UpdatedAt
		var sortExpressions = new Dictionary<string, Expression<Func<T, object>>>
		{
			{ "updated", e => e.UpdatedAt }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}


	/// <inheritdoc />
	public virtual IQueryable<T> ApplyPaging(IQueryable<T> queryable, QueryRequest query)
	{
		return queryable.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
	}

	/// <summary>
	///     Sets the sorting mode for the queryable object based on the specified query request and sort expressions.
	/// </summary>
	/// <param name="queryable">The queryable object to sort.</param>
	/// <param name="query">The query request containing sorting criteria.</param>
	/// <param name="sortExpressions">A dictionary of sort expressions.</param>
	/// <returns>The sorted queryable object.</returns>
	protected static IQueryable<T> SetSortingMode(IQueryable<T> queryable,
		QueryRequest query,
		Dictionary<string, Expression<Func<T, object>>> sortExpressions)
	{
		// Check for both SortColumn and SortOrder
		if (string.IsNullOrEmpty(query.SortBy) || string.IsNullOrEmpty(query.SortOrder) ||
		    !sortExpressions.ContainsKey(query.SortBy.ToLower()))
		{
			return queryable;
		}

		// Check if the sort column is in the sort expressions dictionary
		var sortColumn = query.SortBy.ToLower();
		if (sortExpressions.TryGetValue(sortColumn, out var sortExpression))
		{
			queryable = query.SortOrder.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
				? queryable.OrderByDescending(sortExpression)
				: queryable.OrderBy(sortExpression);
		}
		else
		{
			// Default sorting by UpdatedAt
			queryable = queryable.OrderBy(e => e.UpdatedAt);
		}

		return queryable;
	}
}
