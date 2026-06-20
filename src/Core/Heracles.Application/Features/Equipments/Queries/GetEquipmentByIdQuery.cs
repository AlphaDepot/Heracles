using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.Equipments.Queries;

/// <summary>
///     Retrieves a <see cref="Equipment" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="Equipment" /> to retrieve.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetEquipmentByIdQuery(int Id) : Mediator.IRequest<Result<Equipment>>;

/// <summary>
///     Handles the <see cref="GetEquipmentByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentRepository" />.</param>
public class GetEquipmentByIdQueryHandler(IEquipmentRepository repository)
	: Mediator.IRequestHandler<GetEquipmentByIdQuery, Result<Equipment>>
{
	public async ValueTask<Result<Equipment>> Handle(GetEquipmentByIdQuery request, CancellationToken token)
	{
		var equipment = await repository.GetByIdAsync(request.Id, token);

		return equipment is null
			? Result.Fail<Equipment>(ErrorTypes.NotFound)
			: Result.Ok(equipment);
	}
}
