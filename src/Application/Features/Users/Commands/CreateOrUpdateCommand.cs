using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands;

/// <summary>
///     Represents the request to create or update a user.
/// </summary>
/// <param name="UserId"> The user id.</param>
/// <param name="Email"> The email.</param>
/// <param name="IsAdmin"> The admin status.</param>
public record CreateOrUpdateRequest(string UserId, string Email, bool IsAdmin);

/// <summary>
///     Creates or updates a <see cref="User" />
/// </summary>
/// <param name="UserRequest">The <see cref="CreateOrUpdateRequest" /> to create or update.</param>
///  <returns> The <see cref="Result" /> created or updated.</returns>
public record CreateOrUpdateCommand(CreateOrUpdateRequest UserRequest) : IRequest<Result<bool>>;

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
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class CreateOrUpdateCommandHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<CreateOrUpdateCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(CreateOrUpdateCommand request, CancellationToken cancellationToken)
	{
		var userId = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (userId != request.UserRequest.UserId)
		{
			return Result.Failure<bool>(ErrorTypes.Unauthorized);
		}

		var existingUser = await dbContext.Users
			.FirstOrDefaultAsync(u => u.UserId == request.UserRequest.UserId, cancellationToken);


		if (existingUser == null)
		{
			var newUser = new User
			{
				UserId = request.UserRequest.UserId,
				Email = request.UserRequest.Email,
				IsAdmin = request.UserRequest.IsAdmin
			};

			await dbContext.Users.AddAsync(newUser, cancellationToken);
		}
		else
		{
			existingUser.Email = request.UserRequest.Email;
			existingUser.IsAdmin = request.UserRequest.IsAdmin;

			dbContext.Users.Update(existingUser);
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}
}
