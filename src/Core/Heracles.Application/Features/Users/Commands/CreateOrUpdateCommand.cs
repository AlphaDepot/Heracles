using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests.Users;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Users.Commands;

/// <summary>
///     Creates or updates a <see cref="User" />
/// </summary>
/// <param name="UserRequest">The <see cref="CreateOrUpdateRequest" /> to create or update.</param>
/// <returns> The <see cref="Result" /> created or updated.</returns>
public record CreateOrUpdateCommand(CreateOrUpdateRequest UserRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="CreateOrUpdateCommand" />.
/// </summary>
public class CreateOrUpdateCommandValidator : AbstractValidator<CreateOrUpdateCommand>
{
	public CreateOrUpdateCommandValidator()
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
///     Handles the <see cref="CreateOrUpdateCommand" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="userRepo">The <see cref="IUsersRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class CreateOrUpdateCommandHandler(
	IUsersRepository userRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<CreateOrUpdateCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(
		CreateOrUpdateCommand request,
		CancellationToken cancellationToken)
	{
		var validation = await BusinessValidation(request, cancellationToken);
		if (validation.IsFailed)
		{
			return validation;
		}

		var existingUser = await userRepo.GetByUserIdAsync(
			request.UserRequest.UserId,
			cancellationToken);

		if (existingUser == null)
		{
			var newUser = new User
			{
				UserId = request.UserRequest.UserId,
				Email = request.UserRequest.Email,
				IsAdmin = request.UserRequest.IsAdmin
			};

			await userRepo.AddAsync(newUser, cancellationToken);
		}
		else
		{
			existingUser.Email = request.UserRequest.Email;
			existingUser.IsAdmin = request.UserRequest.IsAdmin;
		}

		await userRepo.SaveChangesAsync(cancellationToken);

		return Result.Ok(true);
	}

	private async ValueTask<Result<bool>> BusinessValidation(
		CreateOrUpdateCommand request,
		CancellationToken token)
	{
		// check logged in user
		var userId = currentUser.UserId;
		if (userId != request.UserRequest.UserId)
		{
			return Result.Fail<bool>(ErrorTypes.Unauthorized);
		}

		// no other validation needed — validator handles format rules

		return Result.Ok(true);
	}
}
