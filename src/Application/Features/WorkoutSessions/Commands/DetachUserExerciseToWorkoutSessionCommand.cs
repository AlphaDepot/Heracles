using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Commands;

/// <summary>
///   Request to detach a <see cref="UserExercise" /> from a <see cref="WorkoutSession" />.
/// </summary>
/// <param name="WorkoutSessionId"> The id of the <see cref="WorkoutSession" />.</param>
/// <param name="UserExerciseId"> The id of the <see cref="UserExercise" />.</param>
public record DetachUserExerciseToWorkoutSessionRequest(int WorkoutSessionId, int UserExerciseId);

/// <summary>
///     Detaches a <see cref="UserExercise" /> from a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="WorkoutSessionRequest"> The <see cref="DetachUserExerciseToWorkoutSessionRequest" />.</param>
///  <inheritdoc cref="IRequestHandler{TRequest,TResponse}" />
public record DetachUserExerciseToWorkoutSessionCommand(DetachUserExerciseToWorkoutSessionRequest WorkoutSessionRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="DetachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
public class
	DetachUserExerciseToWorkoutSessionCommandValidator : AbstractValidator<DetachUserExerciseToWorkoutSessionCommand>
{
	public DetachUserExerciseToWorkoutSessionCommandValidator()
	{
		RuleFor(x => x.WorkoutSessionRequest.UserExerciseId)
			.NotEmpty().WithMessage("UserExerciseId is required")
			.GreaterThan(0).WithMessage("UserExerciseId must be greater than 0");
		RuleFor(x => x.WorkoutSessionRequest.WorkoutSessionId)
			.NotEmpty().WithMessage("WorkoutSessionId is required")
			.GreaterThan(0).WithMessage("WorkoutSessionId must be greater than 0");
	}
}

/// <summary>
///     Handles the <see cref="DetachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class DetachUserExerciseToWorkoutSessionCommandHandler(
	AppDbContext dbContext,
	IHttpContextAccessor contextAccessor) : IRequestHandler<DetachUserExerciseToWorkoutSessionCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(DetachUserExerciseToWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, userExercise, workoutSession) = await BusinessValidation(request, cancellationToken);
		if (validation.IsFailure || userExercise == null || workoutSession == null)
		{
			return validation;
		}

		workoutSession.UserExercises?.Remove(userExercise);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($" Failed to detach User Exercise {userExercise.Id}"));
	}

	private async Task<(Result<bool>, UserExercise?, WorkoutSession?)> BusinessValidation(
		DetachUserExerciseToWorkoutSessionCommand request, CancellationToken cancellationToken)
	{
		// check logged in user
		var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId == null)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null, null);
		}

		// check user exercise
		var userExercise = await dbContext.UserExercises.FirstOrDefaultAsync(
			userExercise => userExercise.Id == request.WorkoutSessionRequest.UserExerciseId, cancellationToken);
		if (userExercise == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("User Exercise not found")), null, null);
		}

		// check workout session
		var workoutSession = await dbContext.WorkoutSessions
			.Include(workoutSession => workoutSession.UserExercises)
			.FirstOrDefaultAsync(workoutSession => workoutSession.Id == request.WorkoutSessionRequest.WorkoutSessionId, cancellationToken);

		if (workoutSession == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Workout Session not found")), null, null);
		}

		// check if the user is the owner of the workout session and user exercise
		if (workoutSession.UserId != userId || userExercise.UserId != userId)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null, null);
		}

		// check if the user exercise is already detached from the workout session
		if (workoutSession.UserExercises == null ||
		    workoutSession.UserExercises.All(exercise => exercise.Id != request.WorkoutSessionRequest.UserExerciseId))
		{
			return (
				Result.Failure<bool>(
					ErrorTypes.BadRequestWithMessage("User Exercise is not attached to the Workout Session")), null,
				null);
		}

		return (Result.Success(true), userExercise, workoutSession);
	}
}
