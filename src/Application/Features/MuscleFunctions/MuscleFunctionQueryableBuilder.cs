using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.MuscleFunctions;

/// <summary>
///     Provides methods for building queryable objects for the MuscleFunction entity with filtering and sorting
///     capabilities.
/// </summary>
public class MuscleFunctionQueryableBuilder : QueryableBuilderBase<MuscleFunction>
{
	/// <inheritdoc />
	public override IQueryable<MuscleFunction> ApplyFilter(IQueryable<MuscleFunction> queryable, QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<MuscleFunction> ApplySorting(IQueryable<MuscleFunction> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<MuscleFunction, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "name", e => e.Name }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
