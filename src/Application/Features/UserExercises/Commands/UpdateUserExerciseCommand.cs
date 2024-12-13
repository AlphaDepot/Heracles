using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExercises.Commands;

/// <summary>
///     Represents the request to update a <see cref="UserExercise" />.
/// </summary>
public class UpdateUserExerciseRequest
{
	public int Id { get; set; }
	public required string Concurrency { get; set; }
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
///     Updates a <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="UserExercise">The <see cref="UpdateUserExerciseRequest" /> to update.</param>
public record UpdateUserExerciseCommand(UpdateUserExerciseRequest UserExercise) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateUserExerciseCommand" />.
/// </summary>
public class UpdateUserExerciseCommandValidator : AbstractValidator<UpdateUserExerciseCommand>
{
	public UpdateUserExerciseCommandValidator()
	{
		RuleFor(x => x.UserExercise.Id)
			.NotEmpty().WithMessage("Id is required")
			.GreaterThan(0).WithMessage("Id must be greater than 0");
		RuleFor(x => x.UserExercise.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateUserExerciseCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class UpdateUserExerciseCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UpdateUserExerciseCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateUserExerciseCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, userExercise) = await BusinessValidation(request, cancellationToken);
		if (validationResult.IsFailure || userExercise == null)
		{
			return validationResult;
		}

		var updatedUserExercise = request.UserExercise.MapUpdateRequestToDbEntity(userExercise);
		dbContext.Entry(userExercise).CurrentValues.SetValues(updatedUserExercise);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return  result > 0 ? Result.Success(true) : Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($"Error updating user exercise with id {request.UserExercise.Id}"));
	}

	private async Task<(Result<bool>, UserExercise?)> BusinessValidation(UpdateUserExerciseCommand request,
		CancellationToken cancellationToken)
	{
		// check if the user exercise exists
		var userExercise = await dbContext.UserExercises
			.FirstOrDefaultAsync(u => u.Id == request.UserExercise.Id, cancellationToken);
		if (userExercise == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		// check if the user is authorized to update the user exercise
		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userExercise.UserId != userId)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		if (userExercise.Concurrency != request.UserExercise.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		return (Result.Success(true), userExercise);
	}
}
