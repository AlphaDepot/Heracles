using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseMuscleGroups;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Commands;

/// <summary>
///     Updates an <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="UpdateExerciseMuscleGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateExerciseMuscleGroupCommand(
	UpdateExerciseMuscleGroupRequest ExerciseMuscleGroup,
	bool IsAdmin = true) : Mediator.IRequest<Result<bool>>;

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
/// <param name="repository">The <see cref="IExerciseMuscleGroupsRepository" />.</param>
public class UpdateExerciseMuscleGroupCommandHandler(IExerciseMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<UpdateExerciseMuscleGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateExerciseMuscleGroupCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		// Apply updated values
		entity.FunctionPercentage = request.ExerciseMuscleGroup.FunctionPercentage;

		// Regenerate concurrency token
		entity.Concurrency = Guid.NewGuid().ToString();

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, ExerciseMuscleGroup?)> BusinessValidation(
		UpdateExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var existing = await repository.GetByIdAsync(request.ExerciseMuscleGroup.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		if (existing.Concurrency != request.ExerciseMuscleGroup.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		return (Result.Ok(true), existing);
	}
}
