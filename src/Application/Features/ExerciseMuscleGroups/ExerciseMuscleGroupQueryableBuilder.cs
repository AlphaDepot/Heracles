using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.ExerciseMuscleGroups;

/// <summary>
///     Provides methods for building queryable objects for the EquipmentGroup entity with filtering and sorting
///     capabilities.
/// </summary>
public class ExerciseMuscleGroupQueryableBuilder : QueryableBuilderBase<ExerciseMuscleGroup>
{
	/// <inheritdoc />
	public override IQueryable<ExerciseMuscleGroup> ApplyFilter(IQueryable<ExerciseMuscleGroup> queryable,
		QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.Muscle.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<ExerciseMuscleGroup> ApplySorting(IQueryable<ExerciseMuscleGroup> queryable,
		QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<ExerciseMuscleGroup, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "exercise", e => e.ExerciseTypeId },
			{ "muscle", e => e.Muscle.Name },
			{ "function", e => e.Function.Name },
			{ "percentage", e => e.FunctionPercentage }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
