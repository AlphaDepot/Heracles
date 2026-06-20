using Heracles.Domain.Interfaces;
using Heracles.Shared.Requests;

namespace Heracles.Application.Utilities;

public static class QueryableBuilderExtensions
{
	public static IQueryable<T> Build<T>(
		this QueryableBuilderBase<T> builder,
		IQueryable<T> queryable,
		QueryRequest request)
		where T : IEntity
	{
		queryable = builder.ApplyFilter(queryable, request);
		queryable = builder.ApplySorting(queryable, request);
		queryable = builder.ApplyPaging(queryable, request);

		return queryable;
	}
}
