using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Users.Commands;

public record UpdateUserCommand(UpdateUserRequest UserRequest, bool IsAdmin = true)
	: IRequest<Result<int>>;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(x => x.UserRequest.UserId)
			.NotEmpty().WithMessage("User Id is required.")
			.Length(36).WithMessage("User Id must be the 36 characters guid.");

		RuleFor(x => x.UserRequest.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Email is not valid.")
			.MaximumLength(255).WithMessage("Email must not exceed 255 characters.");
	}
}

/// <summary>
///     Handles the <see cref="UpdateUserCommand" />.
/// </summary>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
public class UpdateUserCommandHandler(
	IUsersRepository userRepo)
	: IRequestHandler<UpdateUserCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(
		UpdateUserCommand request,
		CancellationToken cancellationToken)
	{
		var (validation, user) =
			await BusinessValidation(request, cancellationToken);

		if (validation.IsFailed || user == null)
		{
			return validation;
		}

		user.Email = request.UserRequest.Email;
		user.IsAdmin = request.UserRequest.IsAdmin;

		await userRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(user.Id);
	}

	private async Task<(Result<int>, User?)> BusinessValidation(
		UpdateUserCommand request,
		CancellationToken token)
	{
		// Only admins can update users unless explicitly allowed
		if (!request.IsAdmin)
		{
			return (Result.Fail<int>(ErrorTypes.Unauthorized), null);
		}

		// Load user
		var user = await userRepo.GetByUserIdAsync(request.UserRequest.UserId, token);

		if (user == null)
		{
			return (Result.Fail<int>(ErrorTypes.NotFound), null);
		}

		return (Result.Ok(0), user);
	}
}
