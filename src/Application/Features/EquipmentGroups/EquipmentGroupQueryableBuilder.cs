using System.Linq.Expressions;
using Application.Common.Requests;
using Application.Common.Utilities;

namespace Application.Features.EquipmentGroups;

/// <summary>
///     Provides methods for building queryable objects for the EquipmentGroup entity with filtering and sorting
///     capabilities.
/// </summary>
public class EquipmentGroupQueryableBuilder : QueryableBuilderBase<EquipmentGroup>
{
	/// <inheritdoc />
	public override IQueryable<EquipmentGroup> ApplyFilter(IQueryable<EquipmentGroup> queryable, QueryRequest query)
	{
		return query.SearchTerm == null
			? queryable
			: queryable.Where(e => e.Name.Contains(query.SearchTerm));
	}

	/// <inheritdoc />
	public override IQueryable<EquipmentGroup> ApplySorting(IQueryable<EquipmentGroup> queryable, QueryRequest query)
	{
		var sortExpressions = new Dictionary<string, Expression<Func<EquipmentGroup, object>>>
		{
			{ "id", e => e.Id },
			{ "created", e => e.CreatedAt },
			{ "updated", e => e.UpdatedAt },
			{ "name", e => e.Name }
		};

		return SetSortingMode(queryable, query, sortExpressions);
	}
}
