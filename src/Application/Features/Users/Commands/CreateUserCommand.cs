using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

/// <summary>
///     Represents the request to create a new <see cref="User" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="UserId"> The unique identifier of the user.</param>
/// <param name="Email"> The email of the user.</param>
/// <param name="IsAdmin"> If true, the user will be created as an admin.</param>
public record CreateUserRequest(string UserId, string Email, bool IsAdmin);

/// <summary>
///     Creates a new <see cref="User" />.
/// </summary>
/// <param name="UserRequest"> The <see cref="CreateUserRequest" /> to create.</param>
/// <param name="IsAdmin"> If true, the command will succeed even if the user is not an admin.</param>
public record CreateUserCommand(CreateUserRequest UserRequest, bool IsAdmin = true) : IRequest<Result<int>>;

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
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
public class CreateUserCommandHandler(AppDbContext dbContext) : IRequestHandler<CreateUserCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var user = request.UserRequest.MapCreateRequestToDbEntity();
		dbContext.Users.Add(user);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(user.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateUserCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		var existingUser = await dbContext.Users.AnyAsync(x => x.UserId == request.UserRequest.UserId);

		return existingUser ? Result.Failure<int>(ErrorTypes.NamingConflict) : Result.Success(0);
	}
}
