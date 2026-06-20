using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.Features.ExerciseMuscleGroups.Queries;

/// <summary>
///     Retrieves a <see cref="ExerciseMuscleGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="ExerciseMuscleGroup" /> group to retrieve.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetExerciseMuscleGroupByIdQuery(int Id)
	: Mediator.IRequest<Result<ExerciseMuscleGroup>>;

/// <summary>
///     Handles the <see cref="GetExerciseMuscleGroupByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IExerciseMuscleGroupsRepository" />.</param>
public class GetExerciseMuscleGroupByIdQueryHandler(IExerciseMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<GetExerciseMuscleGroupByIdQuery, Result<ExerciseMuscleGroup>>
{
	public async ValueTask<Result<ExerciseMuscleGroup>> Handle(
		GetExerciseMuscleGroupByIdQuery request,
		CancellationToken token)
	{
		var entity = await repository.GetByIdAsync(request.Id, token);

		return entity is null
			? Result.Fail<ExerciseMuscleGroup>(ErrorTypes.NotFound)
			: Result.Ok(entity);
	}
}
