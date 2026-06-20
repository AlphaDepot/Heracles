using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.ExerciseTypes.Queries;

/// <summary>
///     Retrieves a list of <see cref="ExerciseType" />s based on a query with optional paging.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A list of <see cref="ExerciseType" />s.</returns>
public record GetPagedExerciseTypesQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<ExerciseType>>>;

/// <summary>
///     Handles the <see cref="GetPagedExerciseTypesQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IExerciseTypesRepository" />.</param>
public class GetPagedExerciseTypesQueryHandler(IExerciseTypesRepository repository)
	: Mediator.IRequestHandler<GetPagedExerciseTypesQuery, Result<PagedResponse<ExerciseType>>>
{
	public async ValueTask<Result<PagedResponse<ExerciseType>>> Handle(
		GetPagedExerciseTypesQuery request,
		CancellationToken token)
	{
		var queryable = new ExerciseTypeQueryableBuilder()
			.Build(repository.Query(), request.Query);

		var result = await queryable.ToListAsync(token);
		var total = await repository.Query().CountAsync(token);

		return PagedResponseFactory.Create(
			result,
			total,
			request.Query.PageNumber,
			request.Query.PageSize
		);
	}

}
