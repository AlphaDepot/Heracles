using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.Equipments;

/// <summary>
///     Provides methods for building queryable objects for the Equipment entity with filtering and sorting capabilities.
/// </summary>
public class EquipmentQueryableBuilder : QueryableBuilderBase<Equipment>
{
	/// <inheritdoc />
	public override IQueryable<Equipment> ApplyFilter(IQueryable<Equipment> queryable, QueryRequest query)
	{
		if (query.SearchTerm == null)
		{
			return queryable;
		}

		var filteredQueryable = queryable.Where(e => e.Type.ToLower().Contains(query.SearchTerm.ToLower()));
		// Check if the filtered queryable is empty
		return !filteredQueryable.Any() ? queryable.Where(e => false) : filteredQueryable;
	}

	/// <inheritdoc />
	public override IQueryable<Equipment> ApplySorting(IQueryable<Equipment> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<Equipment, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "type", e => e.Type },
			{ "weight", e => e.Weight },
			{ "resistance", e => e.Resistance }
		};
		return SetSortingMode(queryable, query, sortExpressions);
	}
}
