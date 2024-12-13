using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExerciseHistories.Queries;

/// <summary>
///     Retrieves a list of <see cref="UserExerciseHistory" />s associated with the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="UserExerciseId"></param>
/// <returns> A <see cref="Result{List}" />.</returns>
public record UserExerciseHistoriesByUserExerciseIdQuery(int UserExerciseId)
	: IRequest<Result<List<UserExerciseHistory>>>;

/// <summary>
///     Handles the <see cref="UserExerciseHistoriesByUserExerciseIdQuery" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
/// <returns> A <see cref="Result{List}" />.</returns>
public class UserExerciseHistoriesByUserExerciseIdQueryHandler(
	AppDbContext dbContext,
	IHttpContextAccessor contextAccessor)
	: IRequestHandler<UserExerciseHistoriesByUserExerciseIdQuery, Result<List<UserExerciseHistory>>>
{
	public async Task<Result<List<UserExerciseHistory>>> Handle(UserExerciseHistoriesByUserExerciseIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (authenticatedUser == null)
		{
			return Result.Failure<List<UserExerciseHistory>>(ErrorTypes.Unauthorized);
		}

		var sessions = await dbContext.UserExerciseHistories
			.Where(x => x.UserId == authenticatedUser && x.UserExerciseId == request.UserExerciseId)
			.OrderBy(x => x.Change)
			.ToListAsync(cancellationToken);

		return Result.Success(sessions);
	}
}
