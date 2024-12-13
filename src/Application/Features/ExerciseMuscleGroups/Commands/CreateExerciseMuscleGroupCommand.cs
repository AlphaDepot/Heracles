using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleGroups;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseMuscleGroups.Commands;

/// <summary>
///     Represents the request to create a new <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <param name="ExerciseTypeId">
///     The ID of the <see cref="ExerciseMuscleGroup" /> to associate with the
///     <see cref="MuscleGroup" />.
/// </param>
/// <param name="MuscleId">
///     The ID of the <see cref="MuscleGroup" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
/// <param name="FunctionId">
///     The ID of the <see cref="MuscleFunction" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
/// <param name="FunctionPercentage">
///     The percentage of the <see cref="MuscleFunction" /> to associate with the
///     <see cref="ExerciseMuscleGroup" />.
/// </param>
public record CreateExerciseMuscleGroupRequest(
	int ExerciseTypeId,
	int MuscleId,
	int FunctionId,
	double FunctionPercentage);

/// <summary>
///     Creates a new <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="CreateExerciseMuscleGroupRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateExerciseMuscleGroupCommand(
	CreateExerciseMuscleGroupRequest ExerciseMuscleGroup,
	bool IsAdmin = true) : IRequest<Result<int>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class CreateExerciseMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<CreateExerciseMuscleGroupCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateExerciseMuscleGroupCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, muscleGroup, muscleFunction) = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var exerciseMuscleGroup = request.ExerciseMuscleGroup.MapCreateRequestToDbEntity(muscleGroup!, muscleFunction!);

		await dbContext.ExerciseMuscleGroups.AddAsync(exerciseMuscleGroup, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(exerciseMuscleGroup.Id);
	}

	private async Task<(Result<int>, MuscleGroup?, MuscleFunction?)> BusinessValidation(
		CreateExerciseMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<int>(ErrorTypes.Unauthorized), null, null);
		}

		// Check if the muscle group and function exist
		var muscleGroup = await dbContext.MuscleGroups.FindAsync(request.ExerciseMuscleGroup.MuscleId);
		if (muscleGroup == null)
		{
			return (Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(MuscleGroup))), null, null);
		}

		// Check if the muscle function exists
		var muscleFunction = await dbContext.MuscleFunctions.FindAsync(request.ExerciseMuscleGroup.FunctionId);
		if (muscleFunction == null)
		{
			return (Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(MuscleFunction))), null, null);
		}

		// Check if the exercise type exists
		var existingExercise = await dbContext.ExerciseTypes.FindAsync(request.ExerciseMuscleGroup.ExerciseTypeId);
		if (existingExercise == null)
		{
			return (Result.Failure<int>(ErrorTypes.NotFoundWithEntityName(nameof(ExerciseTypes))), null, null);
		}

		// Check if the combination of exercise ID, muscle group ID, and muscle function ID is unique
		var existingExerciseMuscleGroup = await dbContext.ExerciseMuscleGroups
			.AnyAsync(x => x.ExerciseTypeId == request.ExerciseMuscleGroup.ExerciseTypeId
			               && x.Muscle == muscleGroup
			               && x.Function == muscleFunction);

		if (existingExerciseMuscleGroup)
		{
			return (
				Result.Failure<int>(
					ErrorTypes.DuplicateEntryWithEntityNames(nameof(ExerciseMuscleGroup), nameof(ExerciseTypes))), null,
				null);
		}

		return (Result.Success(0), muscleGroup, muscleFunction);
	}
}
