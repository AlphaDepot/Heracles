using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserExercises.Commands;

/// <summary>
///     Represents the request to remove a <see cref="UserExercise" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="UserExercise" /> to remove.</param>
public record RemoveUserExerciseCommand(int Id) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveUserExerciseCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class RemoveUserExerciseCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<RemoveUserExerciseCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveUserExerciseCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, userExercise) = await BusinessValidation(request);
		if (validationResult.IsFailure || userExercise == null)
		{
			return validationResult;
		}

		dbContext.UserExercises.Remove(userExercise);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"The UserExercise  with id {request.Id} could not be removed."));
	}

	private async Task<(Result<bool>, UserExercise?)> BusinessValidation(RemoveUserExerciseCommand request)
	{
		var userExercise = await dbContext.UserExercises.FindAsync(request.Id);
		if (userExercise == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId != userExercise.UserId)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Success(true), userExercise);
	}
}
