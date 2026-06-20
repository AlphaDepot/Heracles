using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.UserExerciseHistories.Commands;

/// <summary>
///     Represents the groupRequest to remove a <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="UserExerciseHistory" /> to remove. </param>
public record RemoveUserExerciseHistoryCommand(int Id)
	: IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="historyRepo"> The <see cref="IUserExerciseHistoriesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class RemoveUserExerciseHistoryCommandHandler(
	IUserExerciseHistoriesRepository historyRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<RemoveUserExerciseHistoryCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		RemoveUserExerciseHistoryCommand request,
		CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		await historyRepo.RemoveAsync(entity, token);
		await historyRepo.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExerciseHistory?)> BusinessValidation(
		RemoveUserExerciseHistoryCommand request,
		CancellationToken token)
	{
		var entity = await historyRepo.GetByIdAsync(request.Id, token);
		if (entity is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		// User can only delete their own history
		if (currentUser.UserId != entity.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Ok(true), entity);
	}
}
