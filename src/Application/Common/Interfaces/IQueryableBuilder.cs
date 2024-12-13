using Application.Common.Requests;

namespace Application.Common.Interfaces;

/// <summary>
///     Defines methods for building queryable objects with filtering, sorting, and paging capabilities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IQueryableBuilder<T> where T : IEntity
{
	/// <summary>
	///     Applies a filter to the queryable object based on the specified query request.
	/// </summary>
	/// <param name="queryable">The queryable object to filter.</param>
	/// <param name="query">The query request containing filter criteria.</param>
	/// <returns>The filtered queryable object.</returns>
	IQueryable<T> ApplyFilter(IQueryable<T> queryable, QueryRequest query);

	/// <summary>
	///     Applies sorting to the queryable object based on the specified query request.
	/// </summary>
	/// <param name="queryable">The queryable object to sort.</param>
	/// <param name="query">The query request containing sorting criteria.</param>
	/// <returns>The sorted queryable object.</returns>
	IQueryable<T> ApplySorting(IQueryable<T> queryable, QueryRequest query);

	/// <summary>
	///     Applies paging to the queryable object based on the specified query request.
	/// </summary>
	/// <param name="queryable">The queryable object to page.</param>
	/// <param name="query">The query request containing paging criteria.</param>
	/// <returns>The paged queryable object.</returns>
	IQueryable<T> ApplyPaging(IQueryable<T> queryable, QueryRequest query);
}
