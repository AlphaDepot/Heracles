using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Represents the request to attach an <see cref="Equipment" /> to an <see cref="EquipmentGroup" />.
/// </summary>
/// <param name="EquipmentGroupId"></param>
/// <param name="EquipmentId"></param>
public record AttachEquipmentRequest(int EquipmentGroupId, int EquipmentId);

/// <summary>
///     Attaches an <see cref="Equipment" /> to an <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="EquipmentRequest">The <see cref="AttachEquipmentRequest" /> to attach.</param>
/// <returns>A <see cref="Result{T}" /> with a boolean value indicating success.</returns>
public record AttachEquipmentCommand(AttachEquipmentRequest EquipmentRequest) : IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="AttachEquipmentCommand" />.
/// </summary>
public class AttachEquipmentCommandValidator : AbstractValidator<AttachEquipmentCommand>
{
	public AttachEquipmentCommandValidator()
	{
		RuleFor(x => x.EquipmentRequest.EquipmentGroupId)
			.GreaterThan(0).WithMessage("Equipment Group Id is required");

		RuleFor(x => x.EquipmentRequest.EquipmentId)
			.GreaterThan(0).WithMessage("Equipment Id is required");
	}
}

/// <summary>
///     Handles the <see cref="AttachEquipmentCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class AttachEquipmentCommandHandler(AppDbContext dbContext)
	: IRequestHandler<AttachEquipmentCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(AttachEquipmentCommand request, CancellationToken cancellationToken)
	{
		var (validation, equipmentGroup, equipment) = await BusinessValidation(request);
		if (validation.IsFailure || equipmentGroup == null || equipment == null)
		{
			return validation;
		}

		equipmentGroup.Equipments ??= new List<Equipment>();
		equipmentGroup.Equipments.Add(equipment);

		await dbContext.SaveChangesAsync(cancellationToken);
		return Result.Success(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?, Equipment?)> BusinessValidation(AttachEquipmentCommand request)
	{
		var equipmentGroup = await dbContext.EquipmentGroups.FindAsync(request.EquipmentRequest.EquipmentGroupId);
		if (equipmentGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Equipment Group not found")), null, null);
		}

		var equipment = await dbContext.Equipments.FindAsync(request.EquipmentRequest.EquipmentId);
		if (equipment == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFoundWithMessage("Equipment not found")), null, null);
		}

		return (Result.Success(true), equipmentGroup, equipment);
	}
}
