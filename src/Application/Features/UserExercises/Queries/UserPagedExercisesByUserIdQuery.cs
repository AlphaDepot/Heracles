using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExercises.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="UserExercise" /> related to the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record UserPagedExercisesByUserIdQuery(QueryRequest Query)
	: IRequest<Result<PagedResponse<UserExercise>>>;

/// <summary>
///     Handles the <see cref="UserPagedExercisesByUserIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class UserPagedExercisesByUserIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UserPagedExercisesByUserIdQuery, Result<PagedResponse<UserExercise>>>
{
	public async Task<Result<PagedResponse<UserExercise>>> Handle(UserPagedExercisesByUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (authenticatedUser == null)
		{
			return Result.Failure<PagedResponse<UserExercise>>(ErrorTypes.Unauthorized);
		}


		var queryable = QueryableBuilder(request.Query, authenticatedUser);
		var userExercises = await queryable.ToListAsync(cancellationToken);
		var total = await dbContext.UserExercises.CountAsync(x => x.UserId ==authenticatedUser, cancellationToken);

		return Result.Success(new PagedResponse<UserExercise>
		{
			Data = userExercises,
			PageNumber = request.Query.PageNumber,
			PageSize = request.Query.PageSize,
			TotalPages = (int)Math.Ceiling(total / (double)request.Query.PageSize),
			TotalItems = total
		});
	}

	private IQueryable<UserExercise> QueryableBuilder(QueryRequest query, string userId)
	{
		var queryable = dbContext.UserExercises
			.Include(x => x.ExerciseType)
			.Where(x => x.UserId == userId);

		var builder = new UserExerciseQueryableBuilder();
		queryable = builder.ApplyFilter(queryable, query);
		queryable = builder.ApplySorting(queryable, query);
		queryable = builder.ApplyPaging(queryable, query);

		return queryable;
	}
}
