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
///   Represents the request to create a new <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseId"> The <see cref="UserExerciseHistory.UserExerciseId" /> to create.</param>
/// <param name="Repetition">  The <see cref="UserExerciseHistory.Repetition" /> to create.</param>
/// <param name="UserId"> 	The <see cref="UserExerciseHistory.UserId" /> to create.</param>
public record CreateUserExerciseHistoryRequest(int UserExerciseId, double Weight, int Repetition, string UserId);

/// <summary>
///  Creates a new <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseHistory"> The <see cref="CreateUserExerciseHistoryRequest" /> to create.</param>
/// <param name="IsAdmin"> The <see cref="CreateUserExerciseHistoryRequest" /> to create.</param>
public record CreateUserExerciseHistoryCommand(CreateUserExerciseHistoryRequest UserExerciseHistory, bool IsAdmin = true)
	: IRequest<Result<int>>;


/// <summary>
///   Validates the <see cref="CreateUserExerciseHistoryCommand" />.
/// </summary>
public class CreateUserExerciseHistoryValidator : AbstractValidator<CreateUserExerciseHistoryCommand>
{
	public CreateUserExerciseHistoryValidator()
	{
		RuleFor(x => x.UserExerciseHistory.UserExerciseId).GreaterThan(0).WithMessage("UserExerciseId is required");
		RuleFor(x => x.UserExerciseHistory.Weight).GreaterThanOrEqualTo(0).WithMessage("Weight is required");
		RuleFor(x => x.UserExerciseHistory.Repetition).GreaterThanOrEqualTo(0).WithMessage("Repetition is required");
		RuleFor(x => x.UserExerciseHistory.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");

	}
}

/// <summary>
///  Handles the <see cref="CreateUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class CreateUserExerciseHistoryCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : IRequestHandler<CreateUserExerciseHistoryCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateUserExerciseHistoryCommand request, CancellationToken cancellationToken)
	{
		var businessValidation = await BusinessValidation(request, cancellationToken);
		if (businessValidation.IsFailure)
		{
			return businessValidation;
		}

		var userExerciseHistory = request.UserExerciseHistory.MapCreateRequestToDbEntity();
		await dbContext.UserExerciseHistories.AddAsync(userExerciseHistory, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(userExerciseHistory.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateUserExerciseHistoryCommand request, CancellationToken  cancellationToken)
	{

		// Check if the user exists
		var existingUser = await dbContext.Users
			.AnyAsync(x => x.UserId == request.UserExerciseHistory.UserId, cancellationToken);
		if (!existingUser)
		{
			return Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// check if the userid  is the same as the context userid
		var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId != request.UserExerciseHistory.UserId)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		// check if the user exercise exists
		var existingUserExercise = await dbContext.UserExercises
			.AnyAsync(x => x.Id == request.UserExerciseHistory.UserExerciseId, cancellationToken);
		if (!existingUserExercise)
		{
			return Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(UserExercise)));
		}

		return Result.Success(0);

	}
}
