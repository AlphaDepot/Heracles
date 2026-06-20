using FluentResults;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Interfaces;
using Heracles.Shared.Interfaces.Repositories;

namespace Heracles.Application.Features.MuscleFunctions.Queries;

/// <summary>
///     Retrieves a <see cref="MuscleFunction" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="Mediator.IRequestHandler{TRequest}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="MuscleFunction" /> to retrieve.</param>
/// <returns>A <see cref="Result{MuscleFunction}" />.</returns>
public record GetMuscleFunctionByIdQuery(int Id)
	: Mediator.IRequest<Result<MuscleFunction>>;

/// <summary>
///     Handles the <see cref="GetMuscleFunctionByIdQuery" />.
/// </summary>
/// <param name="repository">The <see cref="IMuscleFunctionsRepository" />.</param>
public class GetMuscleFunctionByIdQueryHandler(IMuscleFunctionsRepository repository)
	: Mediator.IRequestHandler<GetMuscleFunctionByIdQuery, Result<MuscleFunction>>
{
	public async ValueTask<Result<MuscleFunction>> Handle(
		GetMuscleFunctionByIdQuery request,
		CancellationToken token)
	{
		var muscleFunction = await repository.GetByIdAsync(request.Id, token);

		return muscleFunction is null
			? Result.Fail<MuscleFunction>(ErrorTypes.NotFound)
			: Result.Ok(muscleFunction);
	}
}
