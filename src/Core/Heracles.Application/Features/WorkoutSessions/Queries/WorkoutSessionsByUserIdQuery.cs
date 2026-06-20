using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.WorkoutSessions.Queries;

/// <summary>
///     Retrieves a list of <see cref="WorkoutSession" />s associated with the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <returns>A <see cref="Result{List}" />.</returns>
public record WorkoutSessionsByUserIdQuery : IRequest<Result<List<WorkoutSession>>>;

/// <summary>
///     Handles the <see cref="WorkoutSessionsByUserIdQuery" />.
/// </summary>
/// <param name="workoutRepo">The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class WorkoutSessionsByUserIdQueryHandler(
	IWorkoutSessionRepository workoutRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<WorkoutSessionsByUserIdQuery, Result<List<WorkoutSession>>>
{
	public async ValueTask<Result<List<WorkoutSession>>> Handle(
		WorkoutSessionsByUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = currentUser.UserId;

		if (authenticatedUser == null)
		{
			return Result.Fail<List<WorkoutSession>>(ErrorTypes.Unauthorized);
		}

		var sessions = await workoutRepo.Query()
			.Include(x => x.UserExercises)
			.Where(x => x.UserId == authenticatedUser)
			.OrderBy(x => x.DayOfWeek)
			.ThenBy(x => x.SortOrder)
			.ToListAsync(cancellationToken);

		return Result.Ok(sessions);
	}
}
