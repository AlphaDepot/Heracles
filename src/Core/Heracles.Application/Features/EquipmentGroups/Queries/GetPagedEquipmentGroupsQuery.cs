using FluentResults;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.EquipmentGroups.Queries;

/// <summary>
///     Retrieves a page of <see cref="EquipmentGroup" />s based on a query.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Query">The  <see cref="QueryRequest" /> to use to filter the results.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetPagedEquipmentGroupsQuery(QueryRequest Query)
	: Mediator.IRequest<Result<PagedResponse<EquipmentGroup>>>;

/// <summary>
///     Handles the <see cref="GetPagedEquipmentGroupsQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentGroupRepository" />.</param>
public class GetPagedEquipmentGroupsQueryHandler(IEquipmentGroupRepository repository)
	: Mediator.IRequestHandler<GetPagedEquipmentGroupsQuery, Result<PagedResponse<EquipmentGroup>>>
{
	public async ValueTask<Result<PagedResponse<EquipmentGroup>>> Handle(
		GetPagedEquipmentGroupsQuery request,
		CancellationToken token)
	{
		var queryable = new EquipmentGroupQueryableBuilder()
			.Build(repository.Query(), request.Query);

		var result = await queryable.ToListAsync(token);
		var total = await repository.Query().CountAsync(token);

		return Result.Ok(new PagedResponse<EquipmentGroup>
		{
			Data = result,
			PageNumber = request.Query.PageNumber ?? 1,
			PageSize = request.Query.PageSize ?? 10,
			TotalPages = (int)Math.Ceiling(total / (double)(request.Query.PageSize ?? 10)),
			TotalItems = total
		});
	}

}
