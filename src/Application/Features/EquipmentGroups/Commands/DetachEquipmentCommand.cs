using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EquipmentGroups.Commands;

public record DetachEquipmentRequest(int EquipmentGroupId, int EquipmentId);

/// <summary>
///     Detaches an <see cref="Equipment" /> from an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="EquipmentRequest">The <see cref="DetachEquipmentRequest" /> to detach.</param>
///  <returns>A <see cref="Result{T}" /> with a boolean value indicating success.</returns>
public record DetachEquipmentCommand(DetachEquipmentRequest EquipmentRequest)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="DetachEquipmentCommand" />.
/// </summary>
public class DetachEquipmentCommandValidator : AbstractValidator<DetachEquipmentCommand>
{
	public DetachEquipmentCommandValidator()
	{
		RuleFor(x => x.EquipmentRequest.EquipmentGroupId)
			.GreaterThan(0).WithMessage("Equipment Group Id is required");

		RuleFor(x => x.EquipmentRequest.EquipmentId)
			.GreaterThan(0).WithMessage("Equipment Id is required");
	}
}

/// <summary>
///     Handles the <see cref="DetachEquipmentCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class DetachEquipmentCommandHandler(AppDbContext dbContext)
	: IRequestHandler<DetachEquipmentCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(DetachEquipmentCommand request, CancellationToken cancellationToken)
	{
		var (validation, equipmentGroup, equipment) = await BusinessValidation(request);
		if (validation.IsFailure || equipmentGroup == null || equipment == null)
		{
			return validation;
		}

		equipmentGroup.Equipments?.Remove(equipment);

		await dbContext.SaveChangesAsync(cancellationToken);
		return Result.Success(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?, Equipment?)> BusinessValidation(DetachEquipmentCommand request)
	{
		var equipmentGroup = await dbContext.EquipmentGroups
			.Include(eg => eg.Equipments)
			.FirstOrDefaultAsync(eg => eg.Id == request.EquipmentRequest.EquipmentGroupId);

		if (equipmentGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Equipment Group not found")), null, null);
		}

		var equipment = await dbContext.Equipments.FindAsync(request.EquipmentRequest.EquipmentId);
		if (equipment == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Equipment not found")), null, null);
		}

		if (equipmentGroup.Equipments == null || !equipmentGroup.Equipments.Contains(equipment))
		{
			return (
				Result.Failure<bool>(ErrorTypes.BadRequestWithMessage("Equipment not attached to equipment group.")),
				null, null);
		}

		return (Result.Success(true), equipmentGroup, equipment);
	}
}
