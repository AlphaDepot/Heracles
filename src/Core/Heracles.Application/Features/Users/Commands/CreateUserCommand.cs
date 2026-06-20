using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Users.Commands;

/// <summary>
///     Creates a new <see cref="User" />.
/// </summary>
/// <param name="UserRequest"> The <see cref="CreateUserRequest" /> to create.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record CreateUserCommand(CreateUserRequest UserRequest, bool IsAdmin = true)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateUserCommand" />.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
	public CreateUserCommandValidator()
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
///     Handles the <see cref="CreateUserCommand" />.
/// </summary>
/// <param name="userRepo"> The <see cref="IUsersRepository" />.</param>
public class CreateUserCommandHandler(
	IUsersRepository userRepo)
	: IRequestHandler<CreateUserCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(
		CreateUserCommand request,
		CancellationToken cancellationToken)
	{
		var validation = await BusinessValidation(request, cancellationToken);
		if (validation.IsFailed)
		{
			return validation;
		}

		var user = request.UserRequest.MapCreateRequestToDbEntity();

		await userRepo.AddAsync(user, cancellationToken);
		await userRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(user.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(
		CreateUserCommand request,
		CancellationToken token)
	{
		// Only admins can create users unless explicitly allowed
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		// Check if user already exists
		var exists = await userRepo.ExistByUserIdAsync(request.UserRequest.UserId, token);

		if (exists)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}
