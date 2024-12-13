using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.MuscleGroups.Commands;

/// <summary>
///     Removes a <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="MuscleGroup" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveMuscleGroupCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveMuscleGroupCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveMuscleGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveMuscleGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveMuscleGroupCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, muscleGroup) = await BusinessValidation(request);
		if (validationResult.IsFailure || muscleGroup == null)
		{
			return validationResult;
		}

		dbContext.MuscleGroups.Remove(muscleGroup);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, MuscleGroup?)> BusinessValidation(RemoveMuscleGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var muscleGroup = await dbContext.MuscleGroups.FindAsync(request.Id);
		if (muscleGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), muscleGroup);
	}
}
