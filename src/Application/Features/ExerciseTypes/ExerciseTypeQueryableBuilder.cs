using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.ExerciseTypes;

/// <summary>
///     Provides methods for building queryable objects for the ExerciseType entity with filtering and sorting
///     capabilities.
/// </summary>
public class ExerciseTypeQueryableBuilder : QueryableBuilderBase<ExerciseType>
{
	/// <inheritdoc />
	public override IQueryable<ExerciseType> ApplyFilter(IQueryable<ExerciseType> queryable, QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<ExerciseType> ApplySorting(IQueryable<ExerciseType> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<ExerciseType, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "name", e => e.Name }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
