using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using Mediator; using FluentResults;
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
	public async ValueTask<Result<bool>> Handle(RemoveUserExerciseHistoryCommand request,
		CancellationToken cancellationToken)
	{
		var (validationResult, userExerciseHistory) = await BusinessValidation(request);
		if (validationResult.IsFailed || userExerciseHistory == null)
		{
			return validationResult;
		}

		dbContext.UserExerciseHistories.Remove(userExerciseHistory);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Ok(true)
			: Result.Fail<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to remove UserExerciseHistory with Id: {request.Id}"));
	}

	private async Task<(Result<bool>, UserExerciseHistory?)> BusinessValidation(
		RemoveUserExerciseHistoryCommand request)
	{
		var userExerciseHistory = await dbContext.UserExerciseHistories.FindAsync(request.Id);
		if (userExerciseHistory == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId != userExerciseHistory.UserId)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		return (Result.Ok(true), userExerciseHistory);
	}
}
