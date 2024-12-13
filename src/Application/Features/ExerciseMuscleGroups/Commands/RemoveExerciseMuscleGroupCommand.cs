using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.ExerciseMuscleGroups.Commands;

/// <summary>
///     Removes an <see cref="ExerciseMuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The ID of the <see cref="ExerciseMuscleGroup" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveExerciseMuscleGroupCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveExerciseMuscleGroupCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveExerciseMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveExerciseMuscleGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveExerciseMuscleGroupCommand request,
		CancellationToken cancellationToken)
	{
		var (validationResult, exerciseMuscleGroup) = await BusinessValidation(request);
		if (validationResult.IsFailure || exerciseMuscleGroup == null)
		{
			return validationResult;
		}

		dbContext.ExerciseMuscleGroups.Remove(exerciseMuscleGroup);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, ExerciseMuscleGroup?)> BusinessValidation(
		RemoveExerciseMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var exerciseMuscleGroup = await dbContext.ExerciseMuscleGroups.FindAsync(request.Id);
		if (exerciseMuscleGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), exerciseMuscleGroup);
	}
}
