using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Removes a <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="MuscleFunction" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveMuscleFunctionCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveMuscleFunctionCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveMuscleFunctionCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveMuscleFunctionCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveMuscleFunctionCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, muscleFunction) = await BusinessValidation(request);
		if (validationResult.IsFailure || muscleFunction == null)
		{
			return validationResult;
		}

		dbContext.MuscleFunctions.Remove(muscleFunction);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, MuscleFunction?)> BusinessValidation(RemoveMuscleFunctionCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var muscleFunction = await dbContext.MuscleFunctions.FindAsync(request.Id);
		if (muscleFunction == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), muscleFunction);
	}
}
