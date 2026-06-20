using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.ExerciseTypes.Queries;

/// <summary>
///     Retrieves a <see cref="ExerciseType" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="ExerciseType" /> to retrieve.</param>
/// <returns>A <see cref="Result{ExerciseType}" />.</returns>
public record GetExerciseTypeByIdQuery(int Id)
	: Mediator.IRequest<Result<ExerciseType>>;

/// <summary>
///     Handles the <see cref="GetExerciseTypeByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IExerciseTypesRepository" />.</param>
public class GetExerciseTypeByIdQueryHandler(IExerciseTypesRepository repository)
	: Mediator.IRequestHandler<GetExerciseTypeByIdQuery, Result<ExerciseType>>
{
	public async ValueTask<Result<ExerciseType>> Handle(
		GetExerciseTypeByIdQuery request,
		CancellationToken token)
	{
		var exerciseType = await repository.GetByIdAsync(request.Id, token);

		return exerciseType is null
			? Result.Fail<ExerciseType>(ErrorTypes.NotFound)
			: Result.Ok(exerciseType);
	}
}
