using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.ExerciseMuscleGroups;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Commands;

/// <summary>
///     Creates a new <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="CreateExerciseMuscleGroupRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateExerciseMuscleGroupCommand(
	CreateExerciseMuscleGroupRequest ExerciseMuscleGroup,
	bool IsAdmin = true) : Mediator.IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateExerciseMuscleGroupCommand" />.
/// </summary>
public class CreateExerciseMuscleGroupCommandValidator : AbstractValidator<CreateExerciseMuscleGroupCommand>
{
	public CreateExerciseMuscleGroupCommandValidator()
	{
		RuleFor(x => x.ExerciseMuscleGroup.ExerciseTypeId)
			.NotEmpty().WithMessage("Exercise Type Id is required")
			.GreaterThan(0).WithMessage("Exercise Type Id must be greater than 0");

		RuleFor(x => x.ExerciseMuscleGroup.MuscleId)
			.NotEmpty().WithMessage("Muscle Id is required")
			.GreaterThan(0).WithMessage("Muscle Id must be greater than 0");

		RuleFor(x => x.ExerciseMuscleGroup.FunctionId)
			.NotEmpty().WithMessage("Function Id is required")
			.GreaterThan(0).WithMessage("Function Id must be greater than 0");

		RuleFor(x => x.ExerciseMuscleGroup.FunctionPercentage)
			.NotEmpty().WithMessage("Function Percentage is required")
			.GreaterThan(0).WithMessage("Function Percentage must be greater than 0")
			.LessThanOrEqualTo(100).WithMessage("Function Percentage must be less than or equal to 100");
	}
}

/// <summary>
///     Handles the <see cref="CreateExerciseMuscleGroupCommand" />.
/// </summary>
/// <param name="exerciseMuscleGroupsRepo">The <see cref="IExerciseMuscleGroupsRepository" />.</param>
/// <param name="muscleGroupsRepo">The <see cref="IMuscleGroupsRepository" />.</param>
/// <param name="muscleFunctionsRepo">The <see cref="IMuscleFunctionsRepository" />.</param>
/// <param name="exerciseTypesRepo">The <see cref="IExerciseTypesRepository" />.</param>
public class CreateExerciseMuscleGroupCommandHandler(
	IExerciseMuscleGroupsRepository exerciseMuscleGroupsRepo,
	IMuscleGroupsRepository muscleGroupsRepo,
	IMuscleFunctionsRepository muscleFunctionsRepo,
	IExerciseTypesRepository exerciseTypesRepo)
	: Mediator.IRequestHandler<CreateExerciseMuscleGroupCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateExerciseMuscleGroupCommand request, CancellationToken token)
	{
		var (validation, muscleGroup, muscleFunction) = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var entity = request.ExerciseMuscleGroup.MapCreateRequestToDbEntity(muscleGroup!, muscleFunction!);

		await exerciseMuscleGroupsRepo.AddAsync(entity, token);
		await exerciseMuscleGroupsRepo.SaveChangesAsync(token);

		return Result.Ok(entity.Id);
	}

	private async Task<(Result<int>, MuscleGroup?, MuscleFunction?)> BusinessValidation(
		CreateExerciseMuscleGroupCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<int>(ErrorTypes.Unauthorized), null, null);
		}

		var req = request.ExerciseMuscleGroup;

		// Check if the muscle group exists
		var muscleGroup = await muscleGroupsRepo.GetByIdAsync(req.MuscleId, token);
		if (muscleGroup is null)
		{
			return (Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(MuscleGroup))), null, null);
		}

		// Check if the muscle function exists
		var muscleFunction = await muscleFunctionsRepo.GetByIdAsync(req.FunctionId, token);
		if (muscleFunction is null)
		{
			return (Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(MuscleFunction))), null, null);
		}

		// Check if the exercise type exists
		var exerciseType = await exerciseTypesRepo.GetByIdAsync(req.ExerciseTypeId, token);
		if (exerciseType is null)
		{
			return (Result.Fail<int>(ErrorTypes.NotFoundWithEntityName(nameof(ExerciseType))), null, null);
		}

		// Check if the combination of exercise ID, muscle group ID, and muscle function ID is unique
		var exists = await exerciseMuscleGroupsRepo.CombinationExistsAsync(
			req.ExerciseTypeId,
			req.MuscleId,
			req.FunctionId,
			token);

		if (exists)
		{
			return (
				Result.Fail<int>(
					ErrorTypes.DuplicateEntryWithEntityNames(nameof(ExerciseMuscleGroup), nameof(ExerciseType))),
				null,
				null
			);
		}

		return (Result.Ok(0), muscleGroup, muscleFunction);
	}
}
