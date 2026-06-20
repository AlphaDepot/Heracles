using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.MuscleFunctions.Commands;

/// <summary>
///     Removes a <see cref="MuscleFunction" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="MuscleFunction" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveMuscleFunctionCommand(int Id, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveMuscleFunctionCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleFunctionsRepository" />.</param>
public class RemoveMuscleFunctionCommandHandler(IMuscleFunctionsRepository repository)
	: Mediator.IRequestHandler<RemoveMuscleFunctionCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveMuscleFunctionCommand request, CancellationToken token)
	{
		var (validation, entity) = await BusinessValidation(request, token);
		if (validation.IsFailed || entity is null)
		{
			return validation;
		}

		await repository.RemoveAsync(entity, token);
		await repository.SaveChangesAsync(token);

		return Result.Ok(true);
	}

	private async Task<(Result<bool>, MuscleFunction?)> BusinessValidation(
		RemoveMuscleFunctionCommand request,
		CancellationToken token)
	{
		if (!request.IsAdmin)
		{
			return (Result.Fail<bool>(ErrorTypes.Unauthorized), null);
		}

		var entity = await repository.GetByIdAsync(request.Id, token);
		if (entity is null)
		{
			return (Result.Fail<bool>(ErrorTypes.NotFound), null);
		}

		return (Result.Ok(true), entity);
	}
}
