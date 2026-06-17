using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentResults;
using FluentValidation;
using Mediator; using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Equipments.Commands;

/// <summary>
///     Represents the request to create a new <see cref="Equipment" />.
/// </summary>
/// <param name="Type"> The type of the <see cref="Equipment" />.</param>
/// <param name="Weight"> The weight of the <see cref="Equipment" />.</param>
/// <param name="Resistance"> The resistance of the <see cref="Equipment" />.</param>
public record CreateEquipmentRequest(string Type, double Weight, double Resistance);

/// <summary>
///     Creates a new <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Equipment">The <see cref="CreateEquipmentRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record CreateEquipmentCommand(CreateEquipmentRequest Equipment, bool IsAdmin = true) : IRequest<Result<int>>;

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
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class CreateEquipmentCommandHandler(AppDbContext dbContext)
	: IRequestHandler<CreateEquipmentCommand, Result<int>>
{
	public async ValueTask<Result<int>> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailed)
		{
			return validationResult;
		}

		var equipment = request.Equipment.MapCreateRequestToDbEntity();
		await dbContext.Equipments.AddAsync(equipment, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Ok(equipment.Id);
	}

	private async ValueTask<Result<int>> BusinessValidation(CreateEquipmentCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Fail<int>(ErrorTypes.Unauthorized);
		}

		var existingEquipment = await dbContext.Equipments
			.AnyAsync(x => x.Type == request.Equipment.Type);
		if (existingEquipment)
		{
			return Result.Fail<int>(ErrorTypes.NamingConflict);
		}

		return Result.Ok(0);
	}
}
