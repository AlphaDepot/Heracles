using Application.Common.Errors;
using Application.Features.ExerciseMuscleGroups;
using Application.Infrastructure.Data;
using FluentResults;
using FluentValidation;
using Mediator;

namespace Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Removes a <see cref="ExerciseMuscleGroup" /> from an <see cref="ExerciseType" />.
/// </summary>
/// <param name="ExerciseTypeId"> The Id of the <see cref="ExerciseType" />.</param>
/// <param name="MuscleGroupId"> The Id of the <see cref="ExerciseMuscleGroup" />.</param>
public record AttachExerciseMuscleGroupRequest(int ExerciseTypeId, int MuscleGroupId);

/// <summary>
///     Adds a <see cref="ExerciseMuscleGroup" /> to an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="ExerciseMuscleGroup">The <see cref="AttachExerciseMuscleGroupRequest" />.</param>
public record AttachExerciseMuscleGroupCommand(AttachExerciseMuscleGroupRequest ExerciseMuscleGroup)
	: IRequest<Result<bool>>;

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
/// <param name="dbContext"></param>
public class AttachExerciseMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<AttachExerciseMuscleGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(AttachExerciseMuscleGroupCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, exerciseType, muscleGroup) = await BusinessValidation(request);
		if (validation.IsFailed || exerciseType == null || muscleGroup == null)
		{
			return validation;
		}

		exerciseType.MuscleGroups ??= new List<ExerciseMuscleGroup>();
		exerciseType.MuscleGroups.Add(muscleGroup);


		await dbContext.SaveChangesAsync(cancellationToken);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, ExerciseType?, ExerciseMuscleGroup?)> BusinessValidation(
		AttachExerciseMuscleGroupCommand request)
	{
		var exerciseType = await dbContext.ExerciseTypes.FindAsync(request.ExerciseMuscleGroup.ExerciseTypeId);
		if (exerciseType == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Type not found")), null, null);
		}

		var muscleGroup = await dbContext.ExerciseMuscleGroups.FindAsync(request.ExerciseMuscleGroup.MuscleGroupId);
		if (muscleGroup == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Exercise Muscle Group not found")), null,
				null);
		}

		return (Result.Ok(true), exerciseType, muscleGroup);
	}
}
