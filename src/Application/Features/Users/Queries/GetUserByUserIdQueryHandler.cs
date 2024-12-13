using System.Security.Claims;
using System.Text.Json;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries;

/// <summary>
///     Retrieves a <see cref="User" /> by user id.
/// </summary>
/// <param name="UserId">The id of the user to retrieve.</param>
/// <returns>A <see cref="Result{User}" />.</returns>
public record GetUserByUserIdQuery(string UserId) : IRequest<Result<User>>;

/// <summary>
///     Handles the <see cref="GetUserByUserIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class GetUserByUserIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<GetUserByUserIdQuery, Result<User>>
{
	public async Task<Result<User>> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
	{
		if (!await IsUserAuthorized(request))
		{
			return Result.Failure<User>(ErrorTypes.Unauthorized);
		}

		var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
		return user == null
			? Result.Failure<User>(ErrorTypes.NotFound)
			: Result.Success(user);
	}

	private async Task<bool> IsUserAuthorized(GetUserByUserIdQuery request)
	{
		// Check if the user is an admin or the user is the same as the current user
		// get the current user from the context
		var currentUserId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		// get the context user from the database to check if the user is an admin
		var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserId == currentUserId);
		if (user is { IsAdmin: true })
		{
			return true;
		}

		return request.UserId == currentUserId;
	}
}
