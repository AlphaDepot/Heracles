using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using Mediator; using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Application.Features.WorkoutSessions.Commands;

/// <summary>
///     Represents the request to remove a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="WorkoutSession" /> to remove.</param>
public record RemoveWorkoutSessionCommand(int Id) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveWorkoutSessionCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class RemoveWorkoutSessionCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<RemoveWorkoutSessionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveWorkoutSessionCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, workoutSession) = await BusinessValidation(request, cancellationToken);
		if (validationResult.IsFailed || workoutSession == null)
		{
			return validationResult;
		}

		dbContext.WorkoutSessions.Remove(workoutSession);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Ok(true)
			: Result.Fail<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"The WorkoutSession with id {request.Id} could not be removed."));
	}

	private async Task<(Result<bool>, WorkoutSession?)> BusinessValidation(RemoveWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var workoutSession = await dbContext.WorkoutSessions.FindAsync(request.Id, cancellationToken);
		if (workoutSession == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId != workoutSession.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Ok(true), workoutSession);
	}
}
