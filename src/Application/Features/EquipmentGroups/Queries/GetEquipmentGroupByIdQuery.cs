using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EquipmentGroups.Queries;

/// <summary>
///     Retrieves an <see cref="EquipmentGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="EquipmentGroup" /> to retrieve.</param>
/// <returns>A <see cref="Result{EquipmentGroup}" />.</returns>
public record GetEquipmentGroupByIdQuery(int Id) : IRequest<Result<EquipmentGroup>>;

/// <summary>
///     Handles the <see cref="GetEquipmentGroupByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetEquipmentGroupByIdQueryHandler(AppDbContext dbContext) : IRequestHandler<GetEquipmentGroupByIdQuery, Result<EquipmentGroup>>
{
	public async Task<Result<EquipmentGroup>> Handle(GetEquipmentGroupByIdQuery request,
		CancellationToken cancellationToken)
	{
		var equipmentGroup = await dbContext.EquipmentGroups
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return equipmentGroup == null
			? Result.Failure<EquipmentGroup>(ErrorTypes.NotFound)
			: Result.Success(equipmentGroup);
	}
}
