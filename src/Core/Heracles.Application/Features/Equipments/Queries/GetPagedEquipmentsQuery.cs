using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Equipments.Queries;

/// <summary>
///     Retrieves a paged list of <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetPagedEquipmentsQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<Equipment>>>;

/// <summary>
///     Handles the <see cref="GetPagedEquipmentsQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentRepository" />.</param>
public class GetPagedEquipmentsQueryHandler(IEquipmentRepository repository)
	: Mediator.IRequestHandler<GetPagedEquipmentsQuery, Result<PagedResponse<Equipment>>>
{
	public async ValueTask<Result<PagedResponse<Equipment>>> Handle(
		GetPagedEquipmentsQuery request,
		CancellationToken token)
	{
		var queryable = new EquipmentQueryableBuilder()
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
