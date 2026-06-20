using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.WorkoutSessions.Queries;

/// <summary>
///     Retrieves a <see cref="WorkoutSession" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="WorkoutSession" /> to retrieve.</param>
/// <returns>A <see cref="Result{WorkoutSession}" />.</returns>
public record WorkoutSessionByIdAndUserIdQuery(int Id) : IRequest<Result<WorkoutSession>>;

/// <summary>
///     Handles the <see cref="Application.Features.WorkoutSessions.Queries.WorkoutSessionByIdAndUserIdQueryHandler" />.
/// </summary>
/// <param name="workoutRepo">The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class WorkoutSessionByIdAndUserIdQueryHandler(
	IWorkoutSessionRepository workoutRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<WorkoutSessionByIdAndUserIdQuery, Result<WorkoutSession>>
{
	public async ValueTask<Result<WorkoutSession>> Handle(
		WorkoutSessionByIdAndUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = currentUser.UserId;

		if (authenticatedUser == null)
		{
			return Result.Fail<WorkoutSession>(ErrorTypes.Unauthorized);
		}

		var session = await workoutRepo.GetByIdAsync(request.Id, cancellationToken);

		if (session == null)
		{
			return Result.Fail<WorkoutSession>(ErrorTypes.NotFound);
		}

		if (session.UserId != authenticatedUser)
		{
			return Result.Fail<WorkoutSession>(ErrorTypes.Unauthorized);
		}

		return Result.Ok(session);
	}
}
