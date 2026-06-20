using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.EquipmentGroups;

namespace Heracles.Application.Features.EquipmentGroups.Commands;

/// <summary>
///     Updates an existing <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="EquipmentGroup">The <see cref="UpdateEquipmentGroupRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateEquipmentGroupCommand(UpdateEquipmentGroupRequest EquipmentGroup, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateEquipmentGroupCommand" />.
/// </summary>
public class UpdateEquipmentGroupCommandValidator : AbstractValidator<UpdateEquipmentGroupCommand>
{
	public UpdateEquipmentGroupCommandValidator()
	{
		RuleFor(x => x.EquipmentGroup.Id)
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
/// <param name="repository">The <see cref="IEquipmentGroupRepository" />.</param>
public class UpdateEquipmentGroupCommandHandler(IEquipmentGroupRepository repository)
	: Mediator.IRequestHandler<UpdateEquipmentGroupCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateEquipmentGroupCommand request, CancellationToken token)
	{
		var (validation, equipmentGroup) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipmentGroup is null)
		{
			return validation;
		}

		// Apply updated values
		equipmentGroup.Name = request.EquipmentGroup.Name;
		equipmentGroup.Concurrency = Guid.NewGuid().ToString(); // or however you regenerate concurrency tokens

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, EquipmentGroup?)> BusinessValidation(
		UpdateEquipmentGroupCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		// Load existing group
		var existing = await repository.GetByIdAsync(request.EquipmentGroup.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		// Concurrency check
		if (existing.Concurrency != request.EquipmentGroup.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		// Name uniqueness check
		var nameExists = await repository.NameInUseAsync(request.EquipmentGroup.Name, request.EquipmentGroup.Id, token);

		if (nameExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Ok(true), existing);
	}
}
