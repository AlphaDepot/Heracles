using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EquipmentGroups.Queries;

/// <summary>
///     Retrieves a page of <see cref="EquipmentGroup" />s based on a query.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedEquipmentGroupsQuery(QueryRequest Query) : IRequest<Result<PagedResponse<EquipmentGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedEquipmentGroupsQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedEquipmentGroupsQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedEquipmentGroupsQuery, Result<PagedResponse<EquipmentGroup>>>
{
	public async Task<Result<PagedResponse<EquipmentGroup>>> Handle(GetPagedEquipmentGroupsQuery request,
		CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.EquipmentGroups.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<EquipmentGroup>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<EquipmentGroup> QueryableBuilder(QueryRequest requestQuery)
	{
		IQueryable<EquipmentGroup> queryable = dbContext.EquipmentGroups;

		var builder = new EquipmentGroupQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, requestQuery);
		queryable = builder.ApplySorting(queryable, requestQuery);
		queryable = builder.ApplyPaging(queryable, requestQuery);

		return queryable;
	}
}
