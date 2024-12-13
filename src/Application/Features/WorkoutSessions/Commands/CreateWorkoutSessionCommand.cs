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
///     Represents the request to create a new <see cref="WorkoutSession" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Name">The <see cref="CreateWorkoutSessionRequest.Name" /> to create.</param>
/// <param name="DayOfWeek">The <see cref="CreateWorkoutSessionRequest.DayOfWeek" /> to create.</param>
/// <param name="SortOrder">The <see cref="CreateWorkoutSessionRequest.SortOrder" /> to create.</param>
/// <param name="UserId">The <see cref="CreateWorkoutSessionRequest.UserId" /> to create.</param>
public record CreateWorkoutSessionRequest(string Name, string DayOfWeek, int SortOrder, string UserId)
	: IRequest<Result<int>>;

/// <summary>
///     Creates a new <see cref="WorkoutSession" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="WorkoutSession">The <see cref="CreateWorkoutSessionRequest" /> to create.</param>
public record CreateWorkoutSessionCommand(CreateWorkoutSessionRequest WorkoutSession) : IRequest<Result<int>>;

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
			.Must(dayName => DayOfWeekBuilder.GetDayOfWeek(dayName) != null).WithMessage("DayOfWeek is invalid");
		RuleFor(x => x.WorkoutSession.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateWorkoutSessionCommand" />
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class CreateWorkoutSessionCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<CreateWorkoutSessionCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateWorkoutSessionCommand request, CancellationToken cancellationToken)
	{
		var businessValidation = await BusinessValidation(request, cancellationToken);
		if (businessValidation.IsFailure)
		{
			return businessValidation;
		}

		var workoutSession = request.WorkoutSession.MapCreateRequestToDbEntity();
		await dbContext.WorkoutSessions.AddAsync(workoutSession, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(workoutSession.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateWorkoutSessionCommand request,
		CancellationToken cancellationToken)
	{
		// Check if the user exists
		var existingUser = await dbContext.Users
			.AnyAsync(x => x.UserId == request.WorkoutSession.UserId, cancellationToken);
		if (!existingUser)
		{
			return Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// check if the userid  is the same as the context userid
		var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId != request.WorkoutSession.UserId)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		// check if the workout session name is unique
		var existingWorkoutSession = await dbContext.WorkoutSessions
			.AnyAsync(x => x.UserId == request.WorkoutSession.UserId && x.Name == request.WorkoutSession.Name,
				cancellationToken);
		if (existingWorkoutSession)
		{
			return Result.Failure<int>(ErrorTypes.DuplicateEntryWithEntityNames(nameof(WorkoutSession)));
		}


		return Result.Success(0);
	}
}
