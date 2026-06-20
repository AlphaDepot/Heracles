using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.WorkoutSessions;
using Heracles.Shared.Utilities;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.WorkoutSessions.Commands;

/// <summary>
///     Updates a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="WorkoutSession"> The <see cref="UpdateWorkoutSessionRequest" /> to update.</param>
public record UpdateWorkoutSessionCommand(UpdateWorkoutSessionRequest WorkoutSession)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateWorkoutSessionCommand" />.
/// </summary>
public class UpdateWorkoutSessionCommandValidator : AbstractValidator<UpdateWorkoutSessionCommand>
{
	public UpdateWorkoutSessionCommandValidator()
	{
		RuleFor(x => x.WorkoutSession.Id)
			.NotEmpty().WithMessage("Id is required")
			.GreaterThan(0).WithMessage("Id must be greater than 0");

		RuleFor(x => x.WorkoutSession.Name)
			.NotEmpty().WithMessage("Name is required")
			.Length(3, 255).WithMessage("Name must be between 3 and 255 characters");

		RuleFor(x => x.WorkoutSession.DayOfWeek)
			.NotEmpty().WithMessage("DayOfWeek is required")
			.Must(dayName => dayName != null && DayOfWeekBuilder.GetDayOfWeek(dayName) != null)
			.WithMessage("DayOfWeek is invalid");

		RuleFor(x => x.WorkoutSession.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");

		RuleFor(x => x.WorkoutSession.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateWorkoutSessionCommand" />.
/// </summary>
/// <param name="workoutRepo"> The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
/// <param name="currentUser"> The <see cref="ICurrentUserService" />.</param>
public class UpdateWorkoutSessionCommandHandler(
	IWorkoutSessionRepository workoutRepo,
	IUsersRepository userRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UpdateWorkoutSessionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		UpdateWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, existing) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || existing == null)
		{
			return validation;
		}

		var updated = request.WorkoutSession.MapUpdateRequestToDbEntity();

		existing.Name = updated.Name;
		existing.DayOfWeek = updated.DayOfWeek;
		existing.SortOrder = updated.SortOrder;
		existing.UpdatedAt = DateTime.UtcNow;
		existing.Concurrency = Guid.NewGuid().ToString();

		await workoutRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, WorkoutSession?)> BusinessValidation(
		UpdateWorkoutSessionCommand request,
		CancellationToken token)
	{
		// check if the workout session exists
		var existingWorkoutSession = await workoutRepo.GetByIdAsync(request.WorkoutSession.Id, token);

		if (existingWorkoutSession == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithEntityName(nameof(WorkoutSession))), null);
		}

		// Check if the user exists
		var userExists = await userRepo.GetByUserIdAsync(request.WorkoutSession.UserId, token);
		if (userExists == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithEntityName(nameof(User))), null);
		}

		// check if the userid is the same as the context userid
		if (currentUser.UserId != request.WorkoutSession.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		// check if concurrency is valid
		if (request.WorkoutSession.Concurrency != existingWorkoutSession.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		return (Result.Ok(true), existingWorkoutSession);
	}
}
