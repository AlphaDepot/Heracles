using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.Users.Commands;

/// <summary>
///     Represents the request to remove a <see cref="User" />
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id"> The unique identifier of the user.</param>
/// <param name="IsAdmin"> If true, the user will be removed as an admin.</param>
public record RemoveUserCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="RemoveUserCommand" />.
/// </summary>
/// <param name="dbContext"> The <see cref="AppDbContext" />.</param>
public class RemoveUserCommandHandler(AppDbContext dbContext) : IRequestHandler<RemoveUserCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, user) = await BusinessValidation(request);
		if (validationResult.IsFailure || user == null)
		{
			return validationResult;
		}

		dbContext.Users.Remove(user);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, User?)> BusinessValidation(RemoveUserCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var user = await dbContext.Users.FindAsync(request.Id);
		if (user == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), user);
	}
}
