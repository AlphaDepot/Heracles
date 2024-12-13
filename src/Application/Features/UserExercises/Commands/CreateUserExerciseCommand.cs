using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.Users;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExercises.Commands;

/// <summary>
///     Represents the request to create a new <see cref="UserExercise" />.
/// </summary>
public class CreateUserExerciseRequest
{
	public required string UserId { get; set; }
	public required int ExerciseTypeId { get; set; }
	public double? StaticResistance { get; set; }
	public double? PercentageResistance { get; set; }
	public double? CurrentWeight { get; set; }
	public double? PersonalRecord { get; set; }
	public int? DurationInSeconds { get; set; }
	public int? SortOrder { get; set; }
	public int? Repetitions { get; set; }
	public int? Sets { get; set; }
	public bool? Timed { get; set; }
	public bool? BodyWeight { get; set; }
}

/// <summary>
///     Creates a new <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="UserExercise">The <see cref="CreateUserExerciseRequest" /> to create.</param>
public record CreateUserExerciseCommand(CreateUserExerciseRequest UserExercise) : IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateUserExerciseCommand" />.
/// </summary>
public class CreateUserExerciseCommandValidator : AbstractValidator<CreateUserExerciseCommand>
{
	public CreateUserExerciseCommandValidator()
	{
		RuleFor(x => x.UserExercise.UserId)
			.NotEmpty().WithMessage("UserId is required")
			.Length(36).WithMessage("UserId must be 36 characters");
		RuleFor(x => x.UserExercise.ExerciseTypeId)
			.NotEmpty().WithMessage("ExerciseTypeId is required")
			.GreaterThan(0).WithMessage("ExerciseTypeId must be greater than 0");
	}
}

/// <summary>
///     Handles the <see cref="CreateUserExerciseCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class CreateUserExerciseCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<CreateUserExerciseCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateUserExerciseCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var existingId = await GetExistingUserExerciseById(request);

		var userExercise = request.UserExercise.MapCreateRequestToDbEntity();
		// set the version based on an existing user exercise of the same exercise type
		userExercise.Version = existingId > 0 ? existingId + 1 : 1;

		await dbContext.UserExercises.AddAsync(userExercise, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(userExercise.Id);
	}

	private async Task<int> GetExistingUserExerciseById(CreateUserExerciseCommand request)
	{
		var existingUserExercise = await dbContext.UserExercises
			.FirstOrDefaultAsync(x =>
				x.UserId == request.UserExercise.UserId &&
				x.ExerciseTypeId == request.UserExercise.ExerciseTypeId);

		return existingUserExercise?.Id ?? 0;
	}

	private async Task<Result<int>> BusinessValidation(CreateUserExerciseCommand request)
	{
		// check if the user exists
		var existingUser = await dbContext.Users
			.AnyAsync(x => x.UserId == request.UserExercise.UserId);
		if (!existingUser)
		{
			return Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(User)));
		}

		// check if the user is the same as the current user
		if (request.UserExercise.UserId != contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier))
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		// check if the exercise type exists
		var existingExerciseType = await dbContext.ExerciseTypes
			.AnyAsync(x => x.Id == request.UserExercise.ExerciseTypeId);

		if (!existingExerciseType)
		{
			return Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(ExerciseType)));
		}

		return Result.Success(0);
	}
}
