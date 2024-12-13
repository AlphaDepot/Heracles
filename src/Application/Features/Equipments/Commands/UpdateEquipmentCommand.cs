using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Equipments.Commands;

/// <summary>
///     Represents the request to update an <see cref="Equipment" />.
/// </summary>
/// <param name="Id">The Id of the <see cref="Equipment" /> to update.</param>
/// <param name="Type">The type of the <see cref="Equipment" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="Equipment" />.</param>
/// <param name="Weight">The weight of the <see cref="Equipment" />.</param>
/// <param name="Resistance">The resistance of the <see cref="Equipment" />.</param>
public record UpdateEquipmentRequest(int Id, string Type, string? Concurrency, double Weight, double Resistance);

/// <summary>
///     Updates an <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Equipment">The <see cref="UpdateEquipmentRequest" /> to update.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record UpdateEquipmentCommand(UpdateEquipmentRequest Equipment, bool IsAdmin = true) : IRequest<Result<bool>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class UpdateEquipmentCommandHandler(AppDbContext dbContext)
	: IRequestHandler<UpdateEquipmentCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
	{
		var (validation, equipment) = await BusinessValidation(request);
		if (validation.IsFailure || equipment == null)
		{
			return validation;
		}


		var updatedEquipment = request.Equipment.MapUpdateRequestToDbEntity(equipment);
		dbContext.Entry(equipment).CurrentValues.SetValues(updatedEquipment);
		var result = await dbContext.SaveChangesAsync(cancellationToken);

		return result > 0
			? Result.Success(true)
			: Result.Failure<bool>(ErrorTypes.DatabaseErrorWithMessage($"Failed to update Equipment {equipment.Id}"));
	}

	private async Task<(Result<bool>, Equipment?)> BusinessValidation(UpdateEquipmentCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var existingEquipment = await dbContext.Equipments.FindAsync(request.Equipment.Id);
		if (existingEquipment == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		if (existingEquipment.Concurrency != request.Equipment.Concurrency)
		{
			return (Result.Failure<bool>(ErrorTypes.ConcurrencyError), null);
		}

		var nameAlreadyExists = await dbContext.Equipments.AnyAsync(x =>
			x.Type == request.Equipment.Type &&
			x.Id != request.Equipment.Id);

		if (nameAlreadyExists)
		{
			return (Result.Failure<bool>(ErrorTypes.NamingConflict), null);
		}

		return (true, existingEquipment);
	}
}
