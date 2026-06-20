using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.ExerciseTypes.Commands;

/// <summary>
///     Removes an <see cref="ExerciseType" />.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The Id of the <see cref="ExerciseType" /> to remove.</param>
/// <param name="IsAdmin">If true, the command will succeed even if the user is not an admin.</param>
public record RemoveExerciseTypeCommand(int Id, bool IsAdmin = true)
	: Mediator.IRequest<Result<bool>>;

/// <summary>
///     Handles the <see cref="RemoveExerciseTypeCommand" />.
/// </summary>
/// <param name="repository">The <see cref="IExerciseTypesRepository" />.</param>
public class RemoveExerciseTypeCommandHandler(IExerciseTypesRepository repository)
	: Mediator.IRequestHandler<RemoveExerciseTypeCommand, Result<bool>>
{
	public async ValueTask<Result<bool>> Handle(RemoveExerciseTypeCommand request, CancellationToken token)
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

	private async Task<(Result<bool>, ExerciseType?)> BusinessValidation(
		RemoveExerciseTypeCommand request,
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
