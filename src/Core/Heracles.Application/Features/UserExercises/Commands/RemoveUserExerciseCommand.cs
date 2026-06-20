using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.UserExercises.Commands;

/// <summary>
///     Represents the groupRequest to remove a <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="UserExercise" /> to remove.</param>
public record RemoveUserExerciseCommand(int Id) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveUserExerciseCommand" />.
/// </summary>
/// <param name="exerciseRepo">The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class RemoveUserExerciseCommandHandler(
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<RemoveUserExerciseCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveUserExerciseCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		await exerciseRepo.RemoveAsync(entity, token);
		await exerciseRepo.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExercise?)> BusinessValidation(
		RemoveUserExerciseCommand request,
		CancellationToken token)
	{
		var entity = await exerciseRepo.GetByIdAsync(request.Id, token);
		if (entity is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		var userId = currentUser.UserId;
		if (userId != entity.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Ok(true), entity);
	}
}
