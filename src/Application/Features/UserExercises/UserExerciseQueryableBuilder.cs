using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.UserExercises;

/// <summary>
///     Provides methods for building queryable objects for the UserExercise entity with filtering and sorting
///     capabilities.
/// </summary>
public class UserExerciseQueryableBuilder : QueryableBuilderBase<UserExercise>
{
	/// <inheritdoc />
	public override IQueryable<UserExercise> ApplyFilter(IQueryable<UserExercise> queryable, QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.ExerciseType.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<UserExercise> ApplySorting(IQueryable<UserExercise> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<UserExercise, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "name", e => e.ExerciseType.Name },
			{ "weight", e => e.CurrentWeight ?? 0 },
			{ "personal_record", e => e.PersonalRecord ?? 0 },
			{ "duration", e => e.DurationInSeconds },
			{ "sort_order", e => e.SortOrder },
			{ "repetitions", e => e.Repetitions },
			{ "sets", e => e.Sets },
			{ "timed", e => e.Timed },
			{ "body_weight", e => e.BodyWeight }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
