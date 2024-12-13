using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Removes an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="EquipmentGroup" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveEquipmentGroupCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveEquipmentGroupCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveEquipmentGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveEquipmentGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveEquipmentGroupCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, equipmentGroup) = await BusinessValidation(request);
		if (validationResult.IsFailure || equipmentGroup == null)
		{
			return validationResult;
		}

		dbContext.EquipmentGroups.Remove(equipmentGroup);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?)> BusinessValidation(RemoveEquipmentGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var equipmentGroup = await dbContext.EquipmentGroups.FindAsync(request.Id);
		if (equipmentGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), equipmentGroup);
	}
}
