using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Represents the request to update an <see cref="EquipmentGroup" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="EquipmentGroup" /> to update.</param>
/// <param name="Name"> The new name of the <see cref="EquipmentGroup" />.</param>
/// <param name="Concurrency"> The concurrency token of the <see cref="EquipmentGroup" />.</param>
public record UpdateEquipmentGroupRequest(int Id, string Name, string? Concurrency);

/// <summary>
///     Updates an existing <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="EquipmentGroup">The <see cref="UpdateEquipmentGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateEquipmentGroupCommand(UpdateEquipmentGroupRequest EquipmentGroup, bool IsAdmin = true)
	: IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateEquipmentGroupCommand" />.
/// </summary>
public class UpdateEquipmentGroupCommandValidator : AbstractValidator<UpdateEquipmentGroupCommand>
{
	public UpdateEquipmentGroupCommandValidator()
	{
		RuleFor(x => x.EquipmentGroup.Id)
			.NotEmpty().WithMessage("Equipment Group Id is required")
			.GreaterThan(0).WithMessage("Equipment Group Id must be greater than 0");
		RuleFor(x => x.EquipmentGroup.Name)
			.NotEmpty().WithMessage("Equipment Group Name is required")
			.Length(3, 255).WithMessage("Equipment Group Name must be between 3 and 255 characters");
		RuleFor(x => x.EquipmentGroup.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateEquipmentGroupCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class UpdateEquipmentGroupCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateEquipmentGroupCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateEquipmentGroupCommand request, CancellationToken cancellationToken)
	{
		var (validation, equipmentGroup) = await BusinessValidation(request);
		if (validation.IsFailure || equipmentGroup == null)
		{
			return validation;
		}

		var updatedEquipmentGroup = request.EquipmentGroup.MapUpdateRequestToDbEntity(equipmentGroup);
		dbContext.Entry(equipmentGroup).CurrentValues.SetValues(updatedEquipmentGroup);
		var result = await dbContext.SaveChangesAsync(cancellationToken);


		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(
				ErrorTypes.DatabaseErrorWithMessage($"Failed to update Equipment Group {equipmentGroup.Id}"));
	}

	private async Task<(Result<bool>, EquipmentGroup?)> BusinessValidation(UpdateEquipmentGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingEquipmentGroup = await dbContext.EquipmentGroups.FindAsync(request.EquipmentGroup.Id);
		if (existingEquipmentGroup == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingEquipmentGroup.Concurrency != request.EquipmentGroup.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		var nameAlreadyExists = await dbContext.EquipmentGroups.AnyAsync(x =>
			x.Name == request.EquipmentGroup.Name &&
			x.Id != request.EquipmentGroup.Id);

		if (nameAlreadyExists)
		{
			return (Result.Failure<bool>(ErrorTypes.NamingConflict), null);
		}

		return (true, existingEquipmentGroup);
	}
}
