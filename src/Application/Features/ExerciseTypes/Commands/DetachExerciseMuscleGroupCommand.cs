using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Removes a <see cref="ExerciseMuscleGroup" /> from an <see cref="ExerciseType" />.
/// </summary>
/// <param name="ExerciseTypeId"> The Id of the <see cref="ExerciseType" />.</param>
/// <param name="MuscleGroupId"> The Id of the <see cref="ExerciseMuscleGroup" />.</param>
public record DetachExerciseMuscleGroupRequest(int ExerciseTypeId, int MuscleGroupId);

/// <summary>
///     Removes a <see cref="ExerciseMuscleGroup" /> from an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="DetachExerciseMuscleGroupRequest" />.</param>
public record DetachExerciseMuscleGroupCommand(DetachExerciseMuscleGroupRequest ExerciseMuscleGroup)
	: IRequest<Result<bool>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class DetachExerciseMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<DetachExerciseMuscleGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(DetachExerciseMuscleGroupCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, exerciseType, muscleGroup) = await BusinessValidation(request);
		if (validation.IsFailure || exerciseType == null || muscleGroup == null)
		{
			return validation;
		}

		exerciseType.MuscleGroups?.Remove(muscleGroup);

		await dbContext.SaveChangesAsync(cancellationToken);
		return Result.Success(true);
	}

	private async Task<(Result<bool>, ExerciseType?, ExerciseMuscleGroup?)> BusinessValidation(
		DetachExerciseMuscleGroupCommand request)
	{
		var exerciseType = await dbContext.ExerciseTypes
			.Include(et => et.MuscleGroups)
			.FirstOrDefaultAsync(et => et.Id == request.ExerciseMuscleGroup.ExerciseTypeId);

		if (exerciseType == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Exercise Type not found")), null, null);
		}

		var muscleGroup = await dbContext.ExerciseMuscleGroups.FindAsync(request.ExerciseMuscleGroup.MuscleGroupId);
		if (muscleGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Exercise Muscle Group not found")), null,
				null);
		}

		if (exerciseType.MuscleGroups == null || !exerciseType.MuscleGroups.Contains(muscleGroup))
		{
			return (
				Result.Failure<bool>(
					ErrorTypes.BadRequestWithMessage("Exercise Muscle Group not attached to exercise type.")), null,
				null);
		}


		return (Result.Success(true), exerciseType, muscleGroup);
	}
}
