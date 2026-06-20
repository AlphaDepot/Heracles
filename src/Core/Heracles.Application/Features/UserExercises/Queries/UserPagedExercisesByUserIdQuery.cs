using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.UserExercises.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="UserExercise" /> related to the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record UserPagedExercisesByUserIdQuery(QueryRequest Query)
	: IRequest<Result<PagedResponse<UserExercise>>>;

/// <summary>
///     Handles the <see cref="UserPagedExercisesByUserIdQuery" />.
/// </summary>
/// <param name="exerciseRepo">The <see cref="IUserExercisesRepository" />.</param>
/// <param name="currentUser">The <see cref="ICurrentUserService" />.</param>
public class UserPagedExercisesByUserIdQueryHandler(
	IUserExercisesRepository exerciseRepo,
	ICurrentUserService currentUser)
	: IRequestHandler<UserPagedExercisesByUserIdQuery, Result<PagedResponse<UserExercise>>>
{
	public async ValueTask<Result<PagedResponse<UserExercise>>> Handle(
		UserPagedExercisesByUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = currentUser.UserId;

		if (authenticatedUser == null)
		{
			return Result.Fail<PagedResponse<UserExercise>>(ErrorTypes.Unauthorized);
		}

		var queryable = new UserExerciseQueryableBuilder()
			.Build(exerciseRepo.Query(), request.Query);

		var userExercises = await queryable.ToListAsync(cancellationToken);

		var total = await exerciseRepo.Query()
			.CountAsync(x => x.UserId == authenticatedUser, cancellationToken);


		return PagedResponseFactory.Create(
			userExercises,
			total,
			request.Query.PageNumber,
			request.Query.PageSize
		);
	}
}
