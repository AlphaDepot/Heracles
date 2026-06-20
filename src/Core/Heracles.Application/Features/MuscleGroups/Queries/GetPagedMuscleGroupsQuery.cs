using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.MuscleGroups.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="MuscleGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedMuscleGroupsQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<MuscleGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedMuscleGroupsQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleGroupsRepository" />.</param>
public class GetPagedMuscleGroupsQueryHandler(IMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<GetPagedMuscleGroupsQuery, Result<PagedResponse<MuscleGroup>>>
{
	public async ValueTask<Result<PagedResponse<MuscleGroup>>> Handle(
		GetPagedMuscleGroupsQuery request,
		CancellationToken token)
	{
		var queryable = new MuscleGroupQueryableBuilder()
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
