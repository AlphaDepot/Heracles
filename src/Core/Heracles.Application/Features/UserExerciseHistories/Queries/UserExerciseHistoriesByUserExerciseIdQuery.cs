using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.UserExerciseHistories.Queries;

/// <summary>
///     Retrieves a list of <see cref="UserExerciseHistory" />s associated with the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="UserExerciseId"></param>
/// <returns> A <see cref="Result{List}" />.</returns>
public record UserExerciseHistoriesByUserExerciseIdQuery(int UserExerciseId)
	: IRequest<Result<List<UserExerciseHistory>>>;

/// <summary>
///     Handles the <see cref="UserExerciseHistoriesByUserExerciseIdQuery" />.
/// </summary>
/// <param name="historyRepo"> The <see cref="IUserExerciseHistoriesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
/// <returns> A <see cref="Result{List}" />.</returns>
public class UserExerciseHistoriesByUserExerciseIdQueryHandler(
	IUserExerciseHistoriesRepository historyRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UserExerciseHistoriesByUserExerciseIdQuery, Result<List<UserExerciseHistory>>>
{
	public async ValueTask<Result<List<UserExerciseHistory>>> Handle(
		UserExerciseHistoriesByUserExerciseIdQuery request,
		CancellationToken token)
	{
		var authenticatedUser = currentUser.UserId;

		if (authenticatedUser is null)
		{
			return Result.Fail<List<UserExerciseHistory>>(ErrorTypes.Unauthorized);
		}

		var sessions = await historyRepo.Query()
			.Where(x => x.UserId == authenticatedUser && x.UserExerciseId == request.UserExerciseId)
			.OrderBy(x => x.Change)
			.ToListAsync(token);

		return Result.Ok(sessions);
	}
}
