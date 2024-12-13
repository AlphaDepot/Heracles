using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.Users;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExerciseHistories.Commands;

/// <summary>
///   Represents the request to update a <see cref="UserExerciseHistory" />.
/// </summary>
public class UpdateUserExerciseHistoryRequest
{
	public int Id { get; set; }
	public required string Concurrency { get; set; }
	public int UserExerciseId { get; set; }
	public string UserId { get; set; } = null!;
	public double Weight { get; set; }
	public int Repetition { get; set; }


}

/// <summary>
///   Updates a <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseHistory"> The <see cref="UpdateUserExerciseHistoryRequest" /> to update.</param>
public record UpdateUserExerciseHistoryCommand(UpdateUserExerciseHistoryRequest UserExerciseHistory) : IRequest<Result<bool>>;

/// <summary>
///  Validates the <see cref="UpdateUserExerciseHistoryCommand" />.
/// </summary>
public class UpdateUserExerciseHistoryCommandValidator : AbstractValidator<UpdateUserExerciseHistoryCommand>
{
	public UpdateUserExerciseHistoryCommandValidator()
	{
		RuleFor(x => x.UserExerciseHistory.Id)
			.NotEmpty().WithMessage("Id is required")
			.GreaterThan(0).WithMessage("Id must be greater than 0");
		RuleFor(x => x.UserExerciseHistory.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
		RuleFor(x => x.UserExerciseHistory.UserExerciseId)
			.GreaterThan(0).WithMessage("UserExerciseId is required");
		RuleFor(x => x.UserExerciseHistory.Weight)
			.GreaterThanOrEqualTo(0).WithMessage("Weight is required");
		RuleFor(x => x.UserExerciseHistory.Repetition)
			.GreaterThanOrEqualTo(0).WithMessage("Repetition is required");
		RuleFor(x => x.UserExerciseHistory.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");

	}
}


/// <summary>
///   Handles the <see cref="UpdateUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class UpdateUserExerciseHistoryCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UpdateUserExerciseHistoryCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateUserExerciseHistoryCommand request, CancellationToken cancellationToken)
	{

		var (validationResult, userExerciseHistory) = await BusinessValidation(request, cancellationToken);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		userExerciseHistory!.Weight = request.UserExerciseHistory.Weight;
		userExerciseHistory.Repetition = request.UserExerciseHistory.Repetition;
		userExerciseHistory.UpdatedAt = DateTime.UtcNow;

		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return  result > 0 ? Result.Success(true) : Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($"Error updating user exercise history with id {request.UserExerciseHistory.Id}"));

	}

	private async Task<(Result<bool>, UserExerciseHistory?)> BusinessValidation(UpdateUserExerciseHistoryCommand request,
		CancellationToken cancellationToken)
	{
		// check if the user exists
		var existingUser = await dbContext.Users
			.FirstOrDefaultAsync(x => x.UserId == request.UserExerciseHistory.UserId, cancellationToken);

		if (existingUser == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithEntityName(nameof(User))), null);
		}



		// check if the user exercise exists
		var userExercise = await dbContext.UserExercises
			.FirstOrDefaultAsync(u => u.Id == request.UserExerciseHistory.UserExerciseId, cancellationToken);
		if (userExercise == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithEntityName(nameof(UserExercise))), null);
		}

		// check if the user exercise history exists
		var userExerciseHistory = await dbContext.UserExerciseHistories
			.FirstOrDefaultAsync(u => u.Id == request.UserExerciseHistory.Id, cancellationToken);

		if (userExerciseHistory == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithEntityName(nameof(UserExerciseHistory))), null);
		}

		// check if the user is authorized to update the user exercise history
		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userExerciseHistory.UserId != userId || userExercise.UserId != userId  )
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		// validate concurrency
		if (userExerciseHistory.Concurrency != request.UserExerciseHistory.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		return (Result.Success(true), userExerciseHistory);

	}
}

