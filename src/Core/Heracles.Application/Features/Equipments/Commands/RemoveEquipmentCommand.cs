using FluentResults;
using FluentValidation;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.Equipments.Commands;

/// <summary>
///     Removes an <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="Equipment" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveEquipmentCommand(int Id, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveEquipmentCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IEquipmentRepository" />.</param>
public class RemoveEquipmentCommandHandler(IEquipmentRepository repository)
	: Mediator.IRequestHandler<RemoveEquipmentCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveEquipmentCommand request, CancellationToken token)
	{
		var (validation, equipment) = await BusinessValidation(request, token);
		if (validation.IsFailed || equipment is null)
		{
			return validation;
		}

		await repository.RemoveAsync(equipment, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, Equipment?)> BusinessValidation(
		RemoveEquipmentCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var equipment = await repository.GetByIdAsync(request.Id, token);
		if (equipment is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Ok(true), equipment);
	}
}
