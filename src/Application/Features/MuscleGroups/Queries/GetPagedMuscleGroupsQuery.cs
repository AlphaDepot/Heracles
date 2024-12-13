using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleGroups.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedMuscleGroupsQuery(QueryRequest Query) : IRequest<Result<PagedResponse<MuscleGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedMuscleGroupsQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedMuscleGroupsQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedMuscleGroupsQuery, Result<PagedResponse<MuscleGroup>>>
{
	public async Task<Result<PagedResponse<MuscleGroup>>>
		Handle(GetPagedMuscleGroupsQuery request, CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.MuscleGroups.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<MuscleGroup>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<MuscleGroup> QueryableBuilder(QueryRequest requestQuery)
	{
		IQueryable<MuscleGroup> queryable = dbContext.MuscleGroups;

		var builder = new MuscleGroupQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, requestQuery);
		queryable = builder.ApplySorting(queryable, requestQuery);
		queryable = builder.ApplyPaging(queryable, requestQuery);

		return queryable;
	}
}
