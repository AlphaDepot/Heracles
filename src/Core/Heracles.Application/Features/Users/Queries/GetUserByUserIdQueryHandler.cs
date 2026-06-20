using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Users.Queries;

/// <summary>
///     Retrieves a <see cref="User" /> by user id.
/// </summary>
/// <param name="UserId">The id of the user to retrieve.</param>
/// <returns>A <see cref="Result{User}" />.</returns>
public record GetUserByUserIdQuery(string UserId) : IRequest<Result<User>>;

/// <summary>
///     Handles the <see cref="GetUserByUserIdQuery" />.
/// </summary>
/// <param name="userRepo">The <see cref="IUsersRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class GetUserByUserIdQueryHandler(
	IUsersRepository userRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<GetUserByUserIdQuery, Result<User>>
{
	public async ValueTask<Result<User>> Handle(
		GetUserByUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authorized = await IsUserAuthorized(request, cancellationToken);
		if (!authorized)
		{
			return Result.Fail<User>(ErrorTypes.Unauthorized);
		}

		var user = await userRepo.GetByUserIdAsync(
			request.UserId,
			cancellationToken);

		return user == null
			? Result.Fail<User>(ErrorTypes.NotFound)
			: Result.Ok(user);
	}

	private async Task<bool> IsUserAuthorized(
		GetUserByUserIdQuery request,
		CancellationToken token)
	{
		var currentUserId = currentUser.UserId;
		if (currentUserId == null)
		{
			return false;
		}

		// Load current user to check admin status
		var current = await userRepo.GetByUserIdAsync(
			currentUserId,
			token);

		if (current is { IsAdmin: true })
		{
			return true;
		}

		// Otherwise, user can only fetch themselves
		return request.UserId == currentUserId;
	}
}
