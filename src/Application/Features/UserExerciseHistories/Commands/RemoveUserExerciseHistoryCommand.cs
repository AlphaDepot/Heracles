using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.UserExerciseHistories.Commands;

/// <summary>
///     Represents the request to remove a <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="UserExerciseHistory" /> to remove. </param>
public record RemoveUserExerciseHistoryCommand(int Id) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveUserExerciseHistoryCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor"> The <see cref="IHttpContextAccessor" />.</param>
public class RemoveUserExerciseHistoryCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<RemoveUserExerciseHistoryCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveUserExerciseHistoryCommand request,
		CancellationToken cancellationToken)
	{
		var (validationResult, userExerciseHistory) = await BusinessValidation(request);
		if (validationResult.IsFailure || userExerciseHistory == null)
		{
			return validationResult;
		}

		dbContext.UserExerciseHistories.Remove(userExerciseHistory);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to remove UserExerciseHistory with Id: {request.Id}"));
	}

	private async Task<(Result<bool>, UserExerciseHistory?)> BusinessValidation(
		RemoveUserExerciseHistoryCommand request)
	{
		var userExerciseHistory = await dbContext.UserExerciseHistories.FindAsync(request.Id);
		if (userExerciseHistory == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId != userExerciseHistory.UserId)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Success(true), userExerciseHistory);
	}
}
