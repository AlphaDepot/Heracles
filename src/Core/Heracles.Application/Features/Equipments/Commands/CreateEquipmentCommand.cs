using FluentResults;
using FluentValidation;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Requests.Equipments;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.Equipments.Commands;

/// <summary>
///     Creates a new <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Equipment">The <see cref="CreateEquipmentRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateEquipmentCommand(CreateEquipmentRequest Equipment, bool IsAdmin = true)
	: Mediator.IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateEquipmentCommand" />.
/// </summary>
public class CreateEquipmentCommandValidator : AbstractValidator<CreateEquipmentCommand>
{
	public CreateEquipmentCommandValidator()
	{
		RuleFor(x => x.Equipment.Type)
			.NotEmpty().WithMessage("Type is required")
			.Length(1, 255).WithMessage("Type must be between 1 and 255 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateEquipmentCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentRepository" />.</param>
public class CreateEquipmentCommandHandler(IEquipmentRepository repository)
	: Mediator.IRequestHandler<CreateEquipmentCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateEquipmentCommand request, CancellationToken token)
	{
		var validation = await BusinessValidation(request, token);
		if (validation.IsFailed)
		{
			return validation;
		}

		var equipment = request.Equipment.MapCreateRequestToDbEntity();

		await repository.AddAsync(equipment, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(equipment.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(CreateEquipmentCommand request, CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		var exists = await repository.ExistByType(request.Equipment.Type, token);
		if (exists)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}
