using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExerciseHistories.Queries;

public record UserExerciseHistoryByIdQuery(int Id) : IRequest<Result<UserExerciseHistory>>;

public class UserExerciseHistoryByIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UserExerciseHistoryByIdQuery, Result<UserExerciseHistory>>
{
	public async Task<Result<UserExerciseHistory>> Handle(UserExerciseHistoryByIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(authenticatedUser))
		{
			return Result.Failure<UserExerciseHistory>(ErrorTypes.Unauthorized);
		}

		var isAdmin = contextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

		var history = await dbContext.UserExerciseHistories
			.Where(x => x.Id == request.Id)
			.FirstOrDefaultAsync(cancellationToken);

		if (history == null)
		{
			return Result.Failure<UserExerciseHistory>(ErrorTypes.NotFound);
		}

		if (history.UserId != authenticatedUser && !isAdmin)
		{
			return Result.Failure<UserExerciseHistory>(ErrorTypes.Unauthorized);
		}

		return Result.Success(history);
	}
}
