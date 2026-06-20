using FluentResults;
using FluentValidation;
using Heracles.Application.Utilities;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.Equipments;

namespace Heracles.Application.Features.Equipments.Commands;

/// <summary>
///     Updates an <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Equipment">The <see cref="UpdateEquipmentRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateEquipmentCommand(UpdateEquipmentRequest Equipment, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Validates the <see cref="UpdateEquipmentCommand" />.
/// </summary>
public class UpdateEquipmentCommandValidator : AbstractValidator<UpdateEquipmentCommand>
{
	public UpdateEquipmentCommandValidator()
	{
		RuleFor(x => x.Equipment.Id)
			.NotEmpty().WithMessage("Equipment Id is required")
			.GreaterThan(0).WithMessage("Equipment Id must be greater than 0");

		RuleFor(x => x.Equipment.Type)
			.NotEmpty().WithMessage("Equipment Type is required")
			.Length(3, 255).WithMessage("Equipment Type must be between 3 and 255 characters");

		RuleFor(x => x.Equipment.Concurrency)
			.NotEmpty().WithMessage("Concurrency is required")
			.Length(36).WithMessage("Concurrency must be 36 characters");
	}
}

/// <summary>
///     Handles the <see cref="UpdateEquipmentCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentRepository" />.</param>
public class UpdateEquipmentCommandHandler(IEquipmentRepository repository)
	: Mediator.IRequestHandler<UpdateEquipmentCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(UpdateEquipmentCommand request, CancellationToken token)
	{
		var (validation, equipment) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipment is null)
		{
			return validation;
		}

		// Apply updated values
		var updated = request.Equipment.MapUpdateRequestToDbEntity(equipment);
		equipment.Type = updated.Type;
		equipment.Weight = updated.Weight;
		equipment.Resistance = updated.Resistance;

		// Regenerate concurrency token
		equipment.Concurrency = Guid.NewGuid().ToString();

		await repository.SaveChangesAsync(token);
		return Result.Ok(true);
	}

	private async Task<(Result<bool>, Equipment?)> BusinessValidation(
		UpdateEquipmentCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var existing = await repository.GetByIdAsync(request.Equipment.Id, token);
		if (existing is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		if (existing.Concurrency != request.Equipment.Concurrency)
		{
			return (Result.Fail<bool>(ErrorTypes.ConcurrencyAppError), null);
		}

		var nameExists = await repository.NameInUseAsync(request.Equipment.Type, request.Equipment.Id, token);

		if (nameExists)
		{
			return (Result.Fail<bool>(ErrorTypes.NamingConflict), null);
		}

		return (Result.Ok(true), existing);
	}
}
