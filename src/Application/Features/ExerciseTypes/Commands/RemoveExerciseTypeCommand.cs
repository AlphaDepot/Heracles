using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Removes an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="ExerciseType" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveExerciseTypeCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveExerciseTypeCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveExerciseTypeCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveExerciseTypeCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveExerciseTypeCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, exerciseType) = await BusinessValidation(request);
		if (validationResult.IsFailure || exerciseType == null)
		{
			return validationResult;
		}

		dbContext.ExerciseTypes.Remove(exerciseType);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, ExerciseType?)> BusinessValidation(RemoveExerciseTypeCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var exerciseType = await dbContext.ExerciseTypes.FindAsync(request.Id);
		if (exerciseType == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), exerciseType);
	}
}
