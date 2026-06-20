using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseTypes;

namespace Heracles.Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Adds a <see cref="ExerciseMuscleGroup" /> to an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="AttachExerciseMuscleGroupRequest" />.</param>
public record AttachExerciseMuscleGroupCommand(AttachExerciseMuscleGroupRequest ExerciseMuscleGroup)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="AttachExerciseMuscleGroupCommand" />.
/// </summary>
public class AddExerciseMuscleGroupCommandValidator : AbstractValidator<AttachExerciseMuscleGroupCommand>
{
	public AddExerciseMuscleGroupCommandValidator()
	{
		RuleFor(x => x.ExerciseMuscleGroup.ExerciseTypeId).GreaterThan(0);
		RuleFor(x => x.ExerciseMuscleGroup.MuscleGroupId).GreaterThan(0);
	}
}

/// <summary>
///     Handles the <see cref="AttachExerciseMuscleGroupCommand" />.
/// </summary>
/// <param name="exerciseTypesRepo"></param>
/// <param name="exerciseMuscleGroupsRepo"></param>
public class AttachExerciseMuscleGroupCommandHandler(
	IExerciseTypesRepository exerciseTypesRepo,
	IExerciseMuscleGroupsRepository exerciseMuscleGroupsRepo)
	: Mediator.IRequestHandler<AttachExerciseMuscleGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		AttachExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		var (validation, exerciseType, muscleGroup) = await BusinessValidation(request, token);
		if (validation.IsFailed || exerciseType is null || muscleGroup is null)
		{
			return validation;
		}

		exerciseType.MuscleGroups ??= new List<ExerciseMuscleGroup>();
		exerciseType.MuscleGroups.Add(muscleGroup);

		await exerciseTypesRepo.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, ExerciseType?, ExerciseMuscleGroup?)> BusinessValidation(
		AttachExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		var req = request.ExerciseMuscleGroup;

		// Uses overridden GetByIdAsync that includes MuscleGroups
		var exerciseType = await exerciseTypesRepo.GetByIdAsync(req.ExerciseTypeId, token);
		if (exerciseType is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Type not found")), null, null);
		}

		// Load the muscle group normally
		var muscleGroup = await exerciseMuscleGroupsRepo.GetByIdAsync(req.MuscleGroupId, token);
		if (muscleGroup is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Muscle Group not found")), null, null);
		}

		return (Result.Ok(true), exerciseType, muscleGroup);
	}
}
