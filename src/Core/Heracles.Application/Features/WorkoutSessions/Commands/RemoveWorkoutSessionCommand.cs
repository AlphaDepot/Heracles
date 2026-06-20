using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.WorkoutSessions.Commands;

/// <summary>
///     Represents the groupRequest to remove a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="WorkoutSession" /> to remove.</param>
public record RemoveWorkoutSessionCommand(int Id) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveWorkoutSessionCommand" />.
/// </summary>
/// <param name="workoutRepo">The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class RemoveWorkoutSessionCommandHandler(
	IWorkoutSessionRepository workoutRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<RemoveWorkoutSessionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		RemoveWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, workoutSession) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || workoutSession == null)
		{
			return validation;
		}

		await workoutRepo.RemoveAsync(workoutSession, cancellationToken);
		await workoutRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, WorkoutSession?)> BusinessValidation(
		RemoveWorkoutSessionCommand request,
		CancellationToken token)
	{
		// load the workout session
		var workoutSession = await workoutRepo.GetByIdAsync(request.Id, token);
		if (workoutSession == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		// check logged in user
		var userId = currentUser.UserId;
		if (userId != workoutSession.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Ok(true), workoutSession);
	}
}
