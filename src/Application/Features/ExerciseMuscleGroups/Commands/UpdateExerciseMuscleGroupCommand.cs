using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace Application.Features.ExerciseMuscleGroups.Commands;

/// <summary>
///     Represents the request to update an <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <param name="Id"> The ID of the <see cref="ExerciseMuscleGroup" /> to update.</param>
/// <param name="Concurrency"> The concurrency stamp of the <see cref="ExerciseMuscleGroup" /> to update.</param>
/// <param name="FunctionPercentage">
///     The percentage of the <see cref="ExerciseMuscleGroup" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
public record UpdateExerciseMuscleGroupRequest(int Id, string? Concurrency, double FunctionPercentage);

/// <summary>
///     Updates an <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="UpdateExerciseMuscleGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateExerciseMuscleGroupCommand(
	UpdateExerciseMuscleGroupRequest ExerciseMuscleGroup,
	bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateExerciseMuscleGroupCommand" />.
/// </summary>
public class UpdateExerciseMuscleGroupCommandValidator : AbstractValidator<UpdateExerciseMuscleGroupCommand>
{
	public UpdateExerciseMuscleGroupCommandValidator()
	{
		RuleFor(x => x.ExerciseMuscleGroup.Id)
			.NotEmpty().WithMessage("Exercise Muscle Group Id is required")
			.GreaterThan(0).WithMessage("Exercise Muscle Group Id must be greater than 0");
		RuleFor(x => x.ExerciseMuscleGroup.FunctionPercentage)
			.NotEmpty().WithMessage("Function Percentage is required")
			.GreaterThan(0).WithMessage("Function Percentage must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("Function Percentage must be less than or equal to 100");
		RuleFor(x => x.ExerciseMuscleGroup.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateExerciseMuscleGroupCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class UpdateExerciseMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateExerciseMuscleGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateExerciseMuscleGroupCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, exerciseMuscleGroup) = await BusinessValidation(request);
		if (validation.IsFailure || exerciseMuscleGroup == null)
		{
			return validation;
		}


		var updatedExerciseMuscleGroup = request.ExerciseMuscleGroup.MapUpdateRequestToDbEntity(exerciseMuscleGroup);
		dbContext.Entry(exerciseMuscleGroup).CurrentValues.SetValues(updatedExerciseMuscleGroup);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($"Failed to update Exercise Muscle Group {exerciseMuscleGroup.Id}"));
	}

	private async Task<(Result<bool>, ExerciseMuscleGroup?)> BusinessValidation(
		UpdateExerciseMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingExerciseMuscleGroup =
			await dbContext.ExerciseMuscleGroups.FindAsync(request.ExerciseMuscleGroup.Id);
		if (existingExerciseMuscleGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingExerciseMuscleGroup.Concurrency != request.ExerciseMuscleGroup.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		return (Result.Success(true), existingExerciseMuscleGroup);
	}
}
