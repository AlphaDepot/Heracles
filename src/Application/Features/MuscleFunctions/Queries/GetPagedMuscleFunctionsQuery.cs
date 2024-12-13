using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleFunctions.Queries;

/// <summary>
///     Retrieves a page of <see cref="MuscleFunction" />s.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedMuscleFunctionsQuery(QueryRequest Query) : IRequest<Result<PagedResponse<MuscleFunction>>>;

/// <summary>
///     Handles the <see cref="GetPagedMuscleFunctionsQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedMuscleFunctionsQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedMuscleFunctionsQuery, Result<PagedResponse<MuscleFunction>>>
{
	public async Task<Result<PagedResponse<MuscleFunction>>> Handle(GetPagedMuscleFunctionsQuery request,
		CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.MuscleFunctions.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<MuscleFunction>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<MuscleFunction> QueryableBuilder(QueryRequest requestQuery)
	{
		IQueryable<MuscleFunction> queryable = dbContext.MuscleFunctions;

		var builder = new MuscleFunctionQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, requestQuery);
		queryable = builder.ApplySorting(queryable, requestQuery);
		queryable = builder.ApplyPaging(queryable, requestQuery);

		return queryable;
	}
}
