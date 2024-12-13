using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;

namespace Application.Features.Equipments.Commands;

/// <summary>
///     Removes an <see cref="Equipment" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="Equipment" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveEquipmentCommand(int Id, bool IsAdmin = true) : IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveEquipmentCommand" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class RemoveEquipmentCommandHandler(AppDbContext dbContext)
	: IRequestHandler<RemoveEquipmentCommand, Result<bool>>
{
	public async Task<Result<bool>> Handle(RemoveEquipmentCommand request, CancellationToken cancellationToken)
	{
		var (validationResult, equipment) = await BusinessValidation(request);
		if (validationResult.IsFailure || equipment == null)
		{
			return validationResult;
		}

		dbContext.Equipments.Remove(equipment);
		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success(true);
	}

	private async Task<(Result<bool>, Equipment?)> BusinessValidation(RemoveEquipmentCommand request)
	{
		if (!request.IsAdmin)
		{
			return (Result.Failure<bool>(ErrorTypes.Unauthorized), null);
		}

		var equipment = await dbContext.Equipments.FindAsync(request.Id);
		if (equipment == null)
		{
			return (Result.Failure<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Success(true), equipment);
	}
}
