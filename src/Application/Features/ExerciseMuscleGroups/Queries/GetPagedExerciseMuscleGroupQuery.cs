using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseMuscleGroups.Queries;

/// <summary>
///     Retrieves a page of <see cref="ExerciseMuscleGroup" /> records.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedExerciseMuscleGroupQuery(QueryRequest Query)
	: IRequest<Result<PagedResponse<ExerciseMuscleGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedExerciseMuscleGroupQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedExerciseMuscleGroupQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedExerciseMuscleGroupQuery, Result<PagedResponse<ExerciseMuscleGroup>>>
{
	public async Task<Result<PagedResponse<ExerciseMuscleGroup>>>
		Handle(GetPagedExerciseMuscleGroupQuery request, CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.ExerciseMuscleGroups.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<ExerciseMuscleGroup>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<ExerciseMuscleGroup> QueryableBuilder(QueryRequest requestQuery)
	{
		IQueryable<ExerciseMuscleGroup> queryable = dbContext.ExerciseMuscleGroups;

		var builder = new ExerciseMuscleGroupQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, requestQuery);
		queryable = builder.ApplySorting(queryable, requestQuery);
		queryable = builder.ApplyPaging(queryable, requestQuery);

		return queryable;
	}
}
