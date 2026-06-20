using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Users.Commands;

/// <summary>
///     Represents the groupRequest to remove a <see cref="User" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id"> The unique identifier of the user.</param>
/// <param name="IsAdmin"> If true, the user will be removed as an admin.</param>
public record RemoveUserCommand(int Id, bool IsAdmin = true)
	: IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveUserCommand" />.
/// </summary>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
public class RemoveUserCommandHandler(
	IUsersRepository userRepo)
	: IRequestHandler<RemoveUserCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		RemoveUserCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, user) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || user == null)
		{
			return validation;
		}

		await userRepo.RemoveAsync(user, cancellationToken);
		await userRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, User?)> BusinessValidation(
		RemoveUserCommand request,
		CancellationToken token)
	{
		// Only admins can remove users unless explicitly allowed
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		// Load user
		var user = await userRepo.GetByIdAsync(
			request.Id,
			token);

		if (user == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Ok(true), user);
	}
}
