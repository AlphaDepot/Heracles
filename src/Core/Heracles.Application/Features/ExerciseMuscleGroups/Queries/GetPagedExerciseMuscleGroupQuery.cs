using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Queries;

/// <summary>
///     Retrieves a page of <see cref="ExerciseMuscleGroup" /> records.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetPagedExerciseMuscleGroupQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<ExerciseMuscleGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedExerciseMuscleGroupQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IExerciseMuscleGroupsRepository" />.</param>
public class GetPagedExerciseMuscleGroupQueryHandler(IExerciseMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<GetPagedExerciseMuscleGroupQuery, Result<PagedResponse<ExerciseMuscleGroup>>>
{
	public async ValueTask<Result<PagedResponse<ExerciseMuscleGroup>>> Handle(
		GetPagedExerciseMuscleGroupQuery request,
		CancellationToken token)
	{
		var queryable = new ExerciseMuscleGroupQueryableBuilder()
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
