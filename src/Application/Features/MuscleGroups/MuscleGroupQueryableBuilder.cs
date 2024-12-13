using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.MuscleGroups;

/// <summary>
///     Provides methods for building queryable objects for the MuscleGroup entity with filtering and sorting capabilities.
/// </summary>
public class MuscleGroupQueryableBuilder : QueryableBuilderBase<MuscleGroup>
{
	/// <inheritdoc />
	public override IQueryable<MuscleGroup> ApplyFilter(IQueryable<MuscleGroup> queryable, QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<MuscleGroup> ApplySorting(IQueryable<MuscleGroup> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<MuscleGroup, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "name", e => e.Name }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
