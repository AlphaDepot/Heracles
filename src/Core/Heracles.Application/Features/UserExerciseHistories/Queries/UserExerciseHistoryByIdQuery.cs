using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.UserExerciseHistories.Queries;

public record UserExerciseHistoryByIdQuery(int Id)
	: IRequest<Result<UserExerciseHistory>>;

/// <summary>
///     Handles the <see cref="UserExerciseHistoryByIdQuery" />.
/// </summary>
/// <param name="historyRepo"> The <see cref="IUserExerciseHistoriesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class UserExerciseHistoryByIdQueryHandler(
	IUserExerciseHistoriesRepository historyRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UserExerciseHistoryByIdQuery, Result<UserExerciseHistory>>
{
	public async ValueTask<Result<UserExerciseHistory>> Handle(
		UserExerciseHistoryByIdQuery request,
		CancellationToken token)
	{
		var authenticatedUser = currentUser.UserId;
		if (string.IsNullOrEmpty(authenticatedUser))
		{
			return Result.Fail<UserExerciseHistory>(ErrorTypes.Unauthorized);
		}

		var isAdmin = currentUser.IsAuthenticated &&
		              currentUser.UserId != null &&
		              // You likely have a role service; if not, remove admin logic entirely
		              false; // Placeholder if you later add role support

		var history = await historyRepo.GetByIdAsync(request.Id, token);
		if (history is null)
		{
			return Result.Fail<UserExerciseHistory>(ErrorTypes.NotFound);
		}

		if (history.UserId != authenticatedUser && !isAdmin)
		{
			return Result.Fail<UserExerciseHistory>(ErrorTypes.Unauthorized);
		}

		return Result.Ok(history);
	}
}
