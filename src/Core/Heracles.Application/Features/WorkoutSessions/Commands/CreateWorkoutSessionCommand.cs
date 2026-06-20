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
///     Creates a new <see cref="WorkoutSession" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="WorkoutSession">The <see cref="CreateWorkoutSessionRequest" /> to create.</param>
public record CreateWorkoutSessionCommand(CreateWorkoutSessionRequest WorkoutSession)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateWorkoutSessionCommand" />
/// </summary>
public class CreateWorkoutSessionCommandValidator : AbstractValidator<CreateWorkoutSessionCommand>
{
	public CreateWorkoutSessionCommandValidator()
	{
		RuleFor(x => x.WorkoutSession.Name)
			.NotEmpty().WithMessage("Name is required")
			.Length(3, 255).WithMessage("Name must be between 3 and 255 characters");

		RuleFor(x => x.WorkoutSession.DayOfWeek)
			.NotEmpty().WithMessage("DayOfWeek is required")
			.Must(dayName => DayOfWeekBuilder.GetDayOfWeek(dayName) != null)
			.WithMessage("DayOfWeek is invalid");

		RuleFor(x => x.WorkoutSession.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateWorkoutSessionCommand" />
/// </summary>
/// <param name="workoutRepo"> The <see cref="IWorkoutSessionRepository" />.</param>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class CreateWorkoutSessionCommandHandler(
	IWorkoutSessionRepository workoutRepo,
	IUsersRepository userRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<CreateWorkoutSessionCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(
		CreateWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		var validation = await BusinessValidation(request, cancellationToken);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.WorkoutSession.MapCreateRequestToDbEntity();

		await workoutRepo.AddAsync(entity, cancellationToken);
		await workoutRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(entity.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(
		CreateWorkoutSessionCommand request,
		CancellationToken token)
	{
		// Check if the user exists
		var userExists = await userRepo.ExistByUserIdAsync(request.WorkoutSession.UserId, token);
		if (!userExists)
		{
			return Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// check if the userid is the same as the context userid
		if (currentUser.UserId != request.WorkoutSession.UserId)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		// check if the workout session name is unique
		var nameExists = await workoutRepo.NameExistsForUserAsync(request.WorkoutSession.UserId, request.WorkoutSession.Name, token);

		if (nameExists)
		{
			return Result.Fail<int>(ErrorTypes.DuplicateEntryWithEntityNames(nameof(WorkoutSession)));
		}

		return Result.Ok(0);
	}
}
