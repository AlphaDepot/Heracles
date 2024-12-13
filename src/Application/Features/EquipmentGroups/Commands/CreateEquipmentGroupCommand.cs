using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EquipmentGroups.Commands;

public record CreateEquipmentGroupRequest(string Name);

/// <summary>
///     Creates a new <see cref="EquipmentGroup" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="EquipmentGroup">The <see cref="CreateEquipmentGroupRequest" /> to create.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
/// <returns>A <see cref="Result{T}" /> indicating if the operation was successful.</returns>
public record CreateEquipmentGroupCommand(CreateEquipmentGroupRequest EquipmentGroup, bool IsAdmin = true)
	: IRequest<Result<int>>;

/// <summary>
///     Validates the <see cref="CreateEquipmentGroupCommand" />.
/// </summary>
public class CreateEquipmentGroupValidator : AbstractValidator<CreateEquipmentGroupCommand>
{
	public CreateEquipmentGroupValidator()
	{
		RuleFor(x => x.EquipmentGroup.Name)
			.NotEmpty().WithMessage("Name is required")
			.Length(1, 255).WithMessage("Name must be between 1 and 255 characters");
	}
}

/// <summary>
///     Handles the <see cref="CreateEquipmentGroupCommand" />.
/// </summary>
/// <param name="DbContext">The <see cref="AppDbContext" />.</param>
public record CreateEquipmentGroupCommandHandler(AppDbContext DbContext)
	: IRequestHandler<CreateEquipmentGroupCommand, Result<int>>
{
	public async Task<Result<int>> Handle(CreateEquipmentGroupCommand request, CancellationToken cancellationToken)
	{
		var validationResult = await BusinessValidation(request);
		if (validationResult.IsFailure)
		{
			return validationResult;
		}

		var equipmentGroup = request.EquipmentGroup.MapCreateRequestToDbEntity();
		await DbContext.EquipmentGroups.AddAsync(equipmentGroup, cancellationToken);
		await DbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(equipmentGroup.Id);
	}

	private async Task<Result<int>> BusinessValidation(CreateEquipmentGroupCommand request)
	{
		if (!request.IsAdmin)
		{
			return Result.Failure<int>(ErrorTypes.Unauthorized);
		}

		var existingEquipmentGroup = await DbContext.EquipmentGroups
			.AnyAsync(x => x.Name == request.EquipmentGroup.Name);
		if (existingEquipmentGroup)
		{
			return Result.Failure<int>(ErrorTypes.NamingConflict);
		}

		return Result.Success(0);
	}
}
