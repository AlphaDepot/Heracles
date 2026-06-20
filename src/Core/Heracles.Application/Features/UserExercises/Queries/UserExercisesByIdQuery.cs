using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Mediator;

namespace Heracles.Application.Features.UserExercises.Queries;

/// <summary>
///     Retrieves a <see cref="UserExercise" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="UserExercise" /> to retrieve.</param>
/// <returns>A <see cref="Result{UserExercise}" />.</returns>
public record UserExercisesByIdQuery(int Id) : IRequest<Result<UserExercise>>;

/// <summary>
///     Handles the <see cref="UserExercisesByIdQuery" />.
/// </summary>
/// <param name="exerciseRepo">The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class UserExercisesByIdQueryHandler(
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UserExercisesByIdQuery, Result<UserExercise>>
{
	public async ValueTask<Result<UserExercise>> Handle(
		UserExercisesByIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = currentUser.UserId;
		if (authenticatedUser == null)
		{
			return Result.Fail<UserExercise>(ErrorTypes.Unauthorized);
		}

		var userExercise = await exerciseRepo.GetByIdAsync(request.Id, cancellationToken);

		if (userExercise == null)
		{
			return Result.Fail<UserExercise>(ErrorTypes.NotFound);
		}

		if (userExercise.UserId != authenticatedUser)
		{
			return Result.Fail<UserExercise>(ErrorTypes.Unauthorized);
		}

		return Result.Ok(userExercise);
	}
}
