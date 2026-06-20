using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.EquipmentGroups;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Detaches an <see cref="Equipment" /> from an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="EquipmentGroupRequest
/// <see cref="DetachEquipmentGroupRequest" />
/// to detach.
/// </param>
/// <returns>A <see cref="Result" /> with a boolean value indicating success.</returns>
public record DetachEquipmentGroupCommand(DetachEquipmentGroupRequest EquipmentGroupRequest)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="DetachEquipmentGroupCommand" />.
/// </summary>
public class DetachEquipmentCommandValidator : AbstractValidator<DetachEquipmentGroupCommand>
{
	public DetachEquipmentCommandValidator()
	{
		RuleFor(x => x.EquipmentGroupRequest.EquipmentGroupId)
			.GreaterThan(0).WithMessage("Equipment Group Id is required");

		RuleFor(x => x.EquipmentGroupRequest.EquipmentId)
			.GreaterThan(0).WithMessage("Equipment Id is required");
	}
}

/// <summary>
///     Handles the <see cref="DetachEquipmentGroupCommand" />.
/// </summary>
/// <param name="groupRepository">The <see cref="IEquipmentGroupRepository" />.</param>
/// <param name="equipmentRepository">The <see cref="IEquipmentRepository" />.</param>
public class DetachEquipmentCommandHandler(
	IEquipmentGroupRepository groupRepository,
	IEquipmentRepository equipmentRepository)
	: Mediator.IRequestHandler<DetachEquipmentGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(DetachEquipmentGroupCommand request, CancellationToken token)
	{
		var (validation, equipmentGroup, equipment) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipmentGroup is null || equipment is null)
		{
			return validation;
		}

		equipmentGroup.Equipments?.Remove(equipment);

		await groupRepository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?, Equipment?)> BusinessValidation(
		DetachEquipmentGroupCommand request,
		CancellationToken token)
	{
		var groupId = request.EquipmentGroupRequest.EquipmentGroupId;
		var equipmentId = request.EquipmentGroupRequest.EquipmentId;

		//  Load group
		var equipmentGroup = await groupRepository.GetByIdAsync(groupId, token);

		if (equipmentGroup is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Equipment Group not found")), null, null);
		}

		// Load equipment normally
		var equipment = await equipmentRepository.GetByIdAsync(equipmentId, token);
		if (equipment is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Equipment not found")), null, null);
		}

		//  Compare by ID, not reference
		var isAttached = equipmentGroup.Equipments?.Any(e => e.Id == equipment.Id) ?? false;

		if (!isAttached)
		{
			return (
				Result.Fail<bool>(ErrorTypes.BadRequestWithMessage("Equipment not attached to equipment group.")),
				null,
				null
			);
		}

		return (Result.Ok(true), equipmentGroup, equipment);
	}
}
