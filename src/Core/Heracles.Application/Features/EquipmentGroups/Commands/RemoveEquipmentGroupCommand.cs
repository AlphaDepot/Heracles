using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Removes an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="EquipmentGroup" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveEquipmentGroupCommand(int Id, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveEquipmentGroupCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentGroupRepository" />.</param>
public class RemoveEquipmentGroupCommandHandler(IEquipmentGroupRepository repository)
	: Mediator.IRequestHandler<RemoveEquipmentGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveEquipmentGroupCommand request, CancellationToken token)
	{
		var (validation, equipmentGroup) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipmentGroup is null)
		{
			return validation;
		}

		equipmentGroup.Equipments?.Clear();

		await repository.RemoveAsync(equipmentGroup, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?)> BusinessValidation(
		RemoveEquipmentGroupCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var equipmentGroup = await repository.GetByIdAsync(request.Id, token);
		if (equipmentGroup is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Ok(true), equipmentGroup);
	}
}
