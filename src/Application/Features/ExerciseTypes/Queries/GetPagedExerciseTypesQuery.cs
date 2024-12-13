using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseTypes.Queries;

/// <summary>
///     Retrieves a list of <see cref="ExerciseType" />s based on a query with optional paging.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A list of <see cref="ExerciseType" />s.</returns>
public record GetPagedExerciseTypesQuery(QueryRequest Query) : IRequest<Result<PagedResponse<ExerciseType>>>;

/// <summary>
///     Handles the <see cref="GetPagedExerciseTypesQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetPagedExerciseTypesQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetPagedExerciseTypesQuery, Result<PagedResponse<ExerciseType>>>
{
	public async Task<Result<PagedResponse<ExerciseType>>> Handle(GetPagedExerciseTypesQuery request,
		CancellationToken cancellationToken)
	{
		var queryable = QueryableBuilder(request.Query);
		var result = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.ExerciseTypes.CountAsync(cancellationToken);

		return Result.Success(new PagedResponse<ExerciseType>
		{
			Data = result,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize), // convert total items to pages
			TotalItems = total
		});
	}

	private IQueryable<ExerciseType> QueryableBuilder(QueryRequest query)
	{
		IQueryable<ExerciseType> queryable = dbContext.ExerciseTypes;

		var builder = new ExerciseTypeQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, query);
		queryable = builder.ApplySorting(queryable, query);
		queryable = builder.ApplyPaging(queryable, query);
		return queryable;
	}
}
