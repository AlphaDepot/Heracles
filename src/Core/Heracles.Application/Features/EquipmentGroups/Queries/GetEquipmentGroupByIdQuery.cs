using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.EquipmentGroups.Queries;

/// <summary>
///     Retrieves an <see cref="EquipmentGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="EquipmentGroup" /> to retrieve.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetEquipmentGroupByIdQuery(int Id)
	: Mediator.IRequest<Result<EquipmentGroup>>;

/// <summary>
///     Handles the <see cref="GetEquipmentGroupByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentGroupRepository" />.</param>
public class GetEquipmentGroupByIdQueryHandler(IEquipmentGroupRepository repository)
	: Mediator.IRequestHandler<GetEquipmentGroupByIdQuery, Result<EquipmentGroup>>
{
	public async ValueTask<Result<EquipmentGroup>> Handle(
		GetEquipmentGroupByIdQuery request,
		CancellationToken token)
	{
		var equipmentGroup = await repository.GetByIdAsync(request.Id, token);

		return equipmentGroup is null
			? Result.Fail<EquipmentGroup>(ErrorTypes.NotFound)
			: Result.Ok(equipmentGroup);
	}
}
