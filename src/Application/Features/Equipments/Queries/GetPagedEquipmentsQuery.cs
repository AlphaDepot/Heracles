using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Equipments.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedEquipmentsQuery(QueryRequest Query) : IRequest<Result<PagedResponse<Equipment>>>;

/// <summary>
///     Handles the <see cref="GetPagedEquipmentsQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedEquipmentsQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedEquipmentsQuery, Result<PagedResponse<Equipment>>>
{
	public async Task<Result<PagedResponse<Equipment>>>
		Handle(GetPagedEquipmentsQuery request, CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.Equipments.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<Equipment>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<Equipment> QueryableBuilder(QueryRequest requestQuery)
	{
		IQueryable<Equipment> queryable = dbContext.Equipments;
		var builder = new EquipmentQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, requestQuery);
		queryable = builder.ApplySorting(queryable, requestQuery);
		queryable = builder.ApplyPaging(queryable, requestQuery);

		return queryable;
	}
}
