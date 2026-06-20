using FluentResults;
using FluentValidation;
using Heracles.Application.Features.Equipments;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.EquipmentGroups;

namespace Heracles.Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Attaches an <see cref="Equipment" /> to an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="EquipmentGroupRequest">The <see cref="AttachEquipmentGroupRequest" /> to attach.</param>
/// <returns>A <see cref="Result" /> with a boolean value indicating success.</returns>
public record AttachEquipmentCommand(AttachEquipmentGroupRequest EquipmentGroupRequest)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="AttachEquipmentCommand" />.
/// </summary>
public class AttachEquipmentCommandValidator : AbstractValidator<AttachEquipmentCommand>
{
	public AttachEquipmentCommandValidator()
	{
		RuleFor(x => x.EquipmentGroupRequest.EquipmentGroupId)
			.GreaterThan(0).WithMessage("Equipment Group Id is required");

		RuleFor(x => x.EquipmentGroupRequest.EquipmentId)
			.GreaterThan(0).WithMessage("Equipment Id is required");
	}
}

/// <summary>
///     Handles the <see cref="AttachEquipmentCommand" />.
/// </summary>
/// <param name="groupRepository">The <see cref="IEquipmentGroupRepository" />.</param>
/// <param name="equipmentRepository">The <see cref="IEquipmentRepository" />.</param>
public class AttachEquipmentCommandHandler(
	IEquipmentGroupRepository groupRepository,
	IEquipmentRepository equipmentRepository)
	: Mediator.IRequestHandler<AttachEquipmentCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(AttachEquipmentCommand request, CancellationToken token)
	{
		var (validation, equipmentGroup, equipment) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipmentGroup == null || equipment == null)
		{
			return validation;
		}

		equipmentGroup.Equipments ??= new List<Equipment>();
		equipmentGroup.Equipments.Add(equipment);

		await groupRepository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?, Equipment?)> BusinessValidation(AttachEquipmentCommand request,
		CancellationToken token)
	{
		//var equipmentGroup = await repository.GetGroupByIdAsync(groupRequest.EquipmentGroupRequest.EquipmentGroupId, token);
		var equipmentGroup = await groupRepository.GetByIdAsync(request.EquipmentGroupRequest.EquipmentGroupId, token);
		if (equipmentGroup == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Equipment Group not found")), null, null);
		}

		var equipment = await equipmentRepository.GetByIdAsync(request.EquipmentGroupRequest.EquipmentId, token);
		if (equipment == null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFoundWithMessage("Equipment not found")), null, null);
		}

		return (Result.Ok(true), equipmentGroup, equipment);
	}
}
