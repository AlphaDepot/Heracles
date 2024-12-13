using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

/// <summary>
///     Represents the request to update a <see cref="User" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="UserId"> The unique identifier of the user.</param>
/// <param name="Email"> The email of the user.</param>
/// <param name="IsAdmin"> If true, the user will be updated as an admin.</param>
public record UpdateUserRequest(string UserId, string Email, bool IsAdmin);

public record UpdateUserCommand(UpdateUserRequest UserRequest, bool IsAdmin = true) : IRequest<Result<int>>;

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

public class UpdateUserCommandHandler(AppDbContext dbContext) : IRequestHandler<UpdateUserCommand, Result<int>>
{
	public async Task<Result<int>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, currentUser) = await BusinessValidation(request);
		if (validationResult.IsFailure || currentUser == null)
		{
			return validationResult;
		}

		currentUser.Email = request.UserRequest.Email;
		currentUser.IsAdmin = request.UserRequest.IsAdmin;
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(currentUser.Id);
	}

	private async Task<(Result<int>, User?)> BusinessValidation(UpdateUserCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<int>(ErrorTypes.Unauthorized), null);
		}

		var currentUser = await dbContext.Users
			.SingleOrDefaultAsync(u => u.UserId == request.UserRequest.UserId);
		return currentUser == null
			? (Result.Failure<int>(ErrorTypes.NotFound), null)
			: (Result.Success(0), currentUser);
	}
}
