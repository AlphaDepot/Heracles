using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Common.Utilities;
using Application.Features.Users;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Commands;

/// <summary>
///     Represents the request to update a <see cref="WorkoutSession" />.
/// </summary>
public class UpdateWorkoutSessionRequest
{
	public int Id { get; set; }
	public string? Concurrency { get; set; }
	public required string Name { get; set; }
	public string? DayOfWeek { get; set; }
	public int SortOrder { get; set; }
	public required string UserId { get; set; }
}

/// <summary>
///     Updates a <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="WorkoutSession"> The <see cref="UpdateWorkoutSessionRequest" /> to update.</param>
public record UpdateWorkoutSessionCommand(UpdateWorkoutSessionRequest WorkoutSession) : IRequest<Result<bool>>;

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
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class UpdateWorkoutSessionCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UpdateWorkoutSessionCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateWorkoutSessionCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, workoutSession) = await BusinessValidation(request, cancellationToken);
		if (validationResult.IsFailure || workoutSession == null)
		{
			return validationResult;
		}

		var updatedWorkoutSession = request.WorkoutSession.MapUpdateRequestToDbEntity();
		dbContext.Entry(workoutSession).CurrentValues.SetValues(updatedWorkoutSession);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to update Workout Session {workoutSession.Id}"));
	}

	private async Task<(Result<bool>, WorkoutSession?)> BusinessValidation(UpdateWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		// check if the workout session exists
		var existingWorkoutSession = await dbContext.WorkoutSessions
			.FirstOrDefaultAsync(x => x.Id == request.WorkoutSession.Id, cancellationToken);

		if (existingWorkoutSession == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithEntityName(nameof(WorkoutSession))), null);
		}


		// Check if the user exists
		var existingUser = await dbContext.Users
			.AnyAsync(x => x.UserId == request.WorkoutSession.UserId, cancellationToken);
		if (!existingUser)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithEntityName(nameof(User))), null);
		}

		// check if the userid is the same as the context userid
		var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId != request.WorkoutSession.UserId)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		// check if concurrency is valid
		if (request.WorkoutSession.Concurrency != existingWorkoutSession.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}


		return (Result.Success(true), existingWorkoutSession);
	}
}
