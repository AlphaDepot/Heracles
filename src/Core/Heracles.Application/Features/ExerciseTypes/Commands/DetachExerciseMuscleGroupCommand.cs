using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseTypes;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Removes a <see cref="ExerciseMuscleGroup" /> from an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="DetachExerciseMuscleGroupRequest" />.</param>
public record DetachExerciseMuscleGroupCommand(DetachExerciseMuscleGroupRequest ExerciseMuscleGroup)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="DetachExerciseMuscleGroupCommand" />.
/// </summary>
public class DetachExerciseMuscleGroupCommandValidator : AbstractValidator<DetachExerciseMuscleGroupCommand>
{
	public DetachExerciseMuscleGroupCommandValidator()
	{
		RuleFor(x => x.ExerciseMuscleGroup.ExerciseTypeId).GreaterThan(0);
		RuleFor(x => x.ExerciseMuscleGroup.MuscleGroupId).GreaterThan(0);
	}
}

/// <summary>
///     Handles the <see cref="DetachExerciseMuscleGroupCommand" />.
/// </summary>
/// <param name="exerciseTypesRepo">The <see cref="IExerciseTypesRepository" />.</param>
/// <param name="exerciseMuscleGroupsRepo">The <see cref="IExerciseMuscleGroupsRepository" />.</param>
public class DetachExerciseMuscleGroupCommandHandler(
	IExerciseTypesRepository exerciseTypesRepo,
	IExerciseMuscleGroupsRepository exerciseMuscleGroupsRepo)
	: Mediator.IRequestHandler<DetachExerciseMuscleGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		DetachExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		var (validation, exerciseType, muscleGroup) = await BusinessValidation(request, token);
		if (validation.IsFailed || exerciseType is null || muscleGroup is null)
		{
			return validation;
		}

		exerciseType.MuscleGroups?.Remove(muscleGroup);

		await exerciseTypesRepo.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, ExerciseType?, ExerciseMuscleGroup?)> BusinessValidation(
		DetachExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		var req = request.ExerciseMuscleGroup;

		var exerciseType = await exerciseTypesRepo.GetByIdAsync(req.ExerciseTypeId, token);

		if (exerciseType is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Type not found")), null, null);
		}

		var muscleGroup = await exerciseMuscleGroupsRepo.GetByIdAsync(req.MuscleGroupId, token);
		if (muscleGroup is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Muscle Group not found")), null, null);
		}

		if (exerciseType.MuscleGroups is null || !exerciseType.MuscleGroups.Contains(muscleGroup))
		{
			return (
				Result.Fail<bool>(
					ErrorTypes.BadRequestWithMessage("Exercise Muscle Group not attached to exercise type.")),
				null,
				null
			);
		}

		return (Result.Ok(true), exerciseType, muscleGroup);
	}
}
