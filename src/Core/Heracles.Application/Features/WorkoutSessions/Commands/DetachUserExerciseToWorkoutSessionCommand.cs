using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.WorkoutSessions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.WorkoutSessions.Commands;

/// <summary>
///     Detaches a <see cref="UserExercise" /> from a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="WorkoutSessionRequest"> The <see cref="DetachUserExerciseToWorkoutSessionRequest" />.</param>
/// <inheritdoc cref="IRequestHandler{TRequest,TResponse}" />
public record DetachUserExerciseToWorkoutSessionCommand(
	DetachUserExerciseToWorkoutSessionRequest WorkoutSessionRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="DetachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
public class DetachUserExerciseToWorkoutSessionCommandValidator
	: AbstractValidator<DetachUserExerciseToWorkoutSessionCommand>
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
/// <param name="workoutRepo"> The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="exerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class DetachUserExerciseToWorkoutSessionCommandHandler(
	IWorkoutSessionRepository workoutRepo,
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<DetachUserExerciseToWorkoutSessionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		DetachUserExerciseToWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, userExercise, workoutSession) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || userExercise == null || workoutSession == null)
		{
			return validation;
		}

		workoutSession.UserExercises?.Remove(userExercise);

		await workoutRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExercise?, WorkoutSession?)> BusinessValidation(
		DetachUserExerciseToWorkoutSessionCommand request,
		CancellationToken token)
	{
		// check logged in user
		var userId = currentUser.UserId;
		if (userId == null)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null, null);
		}

		// check user exercise
		var userExercise = await exerciseRepo.GetByIdAsync(request.WorkoutSessionRequest.UserExerciseId, token);
		if (userExercise == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("User Exercise not found")), null, null);
		}

		// check workout session
		var workoutSession = await workoutRepo.GetByIdAsync(request.WorkoutSessionRequest.WorkoutSessionId, token);

		if (workoutSession == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Workout Session not found")), null, null);
		}

		// check if the user is the owner of the workout session and user exercise
		if (workoutSession.UserId != userId || userExercise.UserId != userId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null, null);
		}

		// check if the user exercise is already detached from the workout session
		if (workoutSession.UserExercises == null ||
		    workoutSession.UserExercises.All(ex => ex.Id != request.WorkoutSessionRequest.UserExerciseId))
		{
			return (
				Result.Fail<bool>(
					ErrorTypes.BadRequestWithMessage("User Exercise is not attached to the Workout Session")),
				null,
				null);
		}

		return (Result.Ok(true), userExercise, workoutSession);
	}
}
