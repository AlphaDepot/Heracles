using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.MuscleGroups.Queries;

/// <summary>
///     Retrieves a <see cref="MuscleGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="MuscleGroup" /> to retrieve.</param>
/// <returns>A <see cref="Result{MuscleGroup}" />.</returns>
public record GetMuscleGroupByIdQuery(int Id)
	: Mediator.IRequest<Result<MuscleGroup>>;

/// <summary>
///     Handles the <see cref="GetMuscleGroupByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleGroupsRepository" />.</param>
public class GetMuscleGroupByIdQueryHandler(IMuscleGroupsRepository repository)
	: Mediator.IRequestHandler<GetMuscleGroupByIdQuery, Result<MuscleGroup>>
{
	public async ValueTask<Result<MuscleGroup>> Handle(
		GetMuscleGroupByIdQuery request,
		CancellationToken token)
	{
		var muscleGroup = await repository.GetByIdAsync(request.Id, token);

		return muscleGroup is null
			? Result.Fail<MuscleGroup>(ErrorTypes.NotFound)
			: Result.Ok(muscleGroup);
	}
}
