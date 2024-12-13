using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Equipments.Queries;

/// <summary>
///     Retrieves a <see cref="Equipment" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="Equipment" /> to retrieve.</param>
/// <returns>A <see cref="Result{Equipment}" />.</returns>
public record GetEquipmentByIdQuery(int Id) : IRequest<Result<Equipment>>;

/// <summary>
///     Handles the <see cref="GetEquipmentByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetEquipmentByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetEquipmentByIdQuery, Result<Equipment>>
{
	public async Task<Result<Equipment>> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken)
	{
		var equipment = await dbContext.Equipments
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return equipment == null
			? Result.Failure<Equipment>(ErrorTypes.NotFound)
			: Result.Success(equipment);
	}
}
