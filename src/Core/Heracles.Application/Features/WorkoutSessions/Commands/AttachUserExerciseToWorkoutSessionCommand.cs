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
///     Attaches a <see cref="UserExercise" /> to a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="WorkoutSessionRequest"> The <see cref="AttachUserExerciseToWorkoutSessionRequest" />.</param>
/// <returns> A <see cref="Result" /> containing the success status of the operation.</returns>
public record AttachUserExerciseToWorkoutSessionCommand(
	AttachUserExerciseToWorkoutSessionRequest WorkoutSessionRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="AttachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
public class AttachUserExerciseToWorkoutSessionCommandValidator
	: AbstractValidator<AttachUserExerciseToWorkoutSessionCommand>
{
	public AttachUserExerciseToWorkoutSessionCommandValidator()
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
///     Handles the <see cref="AttachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
/// <param name="workoutRepo"> The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="exerciseRepo"> The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class AttachUserExerciseToWorkoutSessionCommandHandler(
	IWorkoutSessionRepository workoutRepo,
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<AttachUserExerciseToWorkoutSessionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		AttachUserExerciseToWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, userExercise, workoutSession) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || userExercise == null || workoutSession == null)
		{
			return validation;
		}

		workoutSession.UserExercises ??= new List<UserExercise>();
		workoutSession.UserExercises.Add(userExercise);

		await workoutRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, UserExercise?, WorkoutSession?)> BusinessValidation(
		AttachUserExerciseToWorkoutSessionCommand request,
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

		// Error if the user exercise is already attached to the workout session
		if (workoutSession.UserExercises != null &&
		    workoutSession.UserExercises.Any(x => x.Id == userExercise.Id))
		{
			return (Result.Fail<bool>(ErrorTypes.DuplicateEntryWithEntityNames(nameof(UserExercise))), null, null);
		}

		return (Result.Ok(true), userExercise, workoutSession);
	}
}
