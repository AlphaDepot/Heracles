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
///   Request to attach a <see cref="UserExercise" /> to a <see cref="WorkoutSession" />.
/// </summary>
/// <param name="UserExerciseId"> The id of the <see cref="UserExercise" /> to attach to the <see cref="WorkoutSession" />.</param>
/// <param name="WorkoutSessionId"> The id of the <see cref="WorkoutSession" /> to attach the <see cref="UserExercise" /> to.</param>
public record AttachUserExerciseToWorkoutSessionRequest(int UserExerciseId, int WorkoutSessionId);

/// <summary>
///     Attaches a <see cref="UserExercise" /> to a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="WorkoutSessionRequest"> The <see cref="AttachUserExerciseToWorkoutSessionRequest" />.</param>
/// <returns> A <see cref="Result" /> containing the success status of the operation.</returns>
public record AttachUserExerciseToWorkoutSessionCommand(AttachUserExerciseToWorkoutSessionRequest WorkoutSessionRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="AttachUserExerciseToWorkoutSessionCommand" />.
/// </summary>
public class
	AttachUserExerciseToWorkoutSessionCommandValidator : AbstractValidator<AttachUserExerciseToWorkoutSessionCommand>
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
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class AttachUserExerciseToWorkoutSessionCommandHandler(
	AppDbContext dbContext,
	IHttpContextAccessor contextAccessor) : IRequestHandler<AttachUserExerciseToWorkoutSessionCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(AttachUserExerciseToWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, userExercise, workoutSession) = await BusinessValidation(request, cancellationToken);
		if (validation.IsFailure || userExercise == null || workoutSession == null)
		{
			return validation;
		}

		workoutSession.UserExercises ??= new List<UserExercise>();
		workoutSession.UserExercises.Add(userExercise);

		await dbContext.SaveChangesAsync(cancellationToken);
		return Result.Success(true);
	}

	private async Task<(Result<bool>, UserExercise?, WorkoutSession?)> BusinessValidation(
		AttachUserExerciseToWorkoutSessionCommand request, CancellationToken cancellationToken)
	{
		// check logged in user
		var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId == null)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null, null);
		}

		// check user exercise
		var userExercise = await dbContext.UserExercises.FindAsync(request.WorkoutSessionRequest.UserExerciseId);
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


		// Error if the user is already attached to the workout session
		if (workoutSession.UserExercises != null && workoutSession.UserExercises.Any(x => x.Id == userExercise.Id))
		{
			return (Result.Failure<bool>(ErrorTypes.DuplicateEntryWithEntityNames(nameof(UserExercise))), null, null);
		}

		return (Result.Success(true), userExercise, workoutSession);
	}
}
