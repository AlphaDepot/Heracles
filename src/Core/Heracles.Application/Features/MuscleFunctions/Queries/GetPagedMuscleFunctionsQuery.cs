using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.MuscleFunctions.Queries;

/// <summary>
///     Retrieves a page of <see cref="MuscleFunction" />s.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result{PagedResponse}" />.</returns>
public record GetPagedMuscleFunctionsQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<MuscleFunction>>>;

/// <summary>
///     Handles the <see cref="GetPagedMuscleFunctionsQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleFunctionsRepository" />.</param>
public class GetPagedMuscleFunctionsQueryHandler(IMuscleFunctionsRepository repository)
	: Mediator.IRequestHandler<GetPagedMuscleFunctionsQuery, Result<PagedResponse<MuscleFunction>>>
{
	public async ValueTask<Result<PagedResponse<MuscleFunction>>> Handle(
		GetPagedMuscleFunctionsQuery request,
		CancellationToken token)
	{
		var queryable = new MuscleFunctionQueryableBuilder()
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
