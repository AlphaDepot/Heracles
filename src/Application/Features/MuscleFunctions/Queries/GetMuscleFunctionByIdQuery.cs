using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleFunctions.Queries;

/// <summary>
///     Retrieves a <see cref="MuscleFunction" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="MuscleFunction" /> to retrieve.</param>
/// <returns>A <see cref="Result{MuscleFunction}" />.</returns>
public record GetMuscleFunctionByIdQuery(int Id) : IRequest<Result<MuscleFunction>>;

/// <summary>
///     Handles the <see cref="GetMuscleFunctionByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetMuscleFunctionByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetMuscleFunctionByIdQuery, Result<MuscleFunction>>
{
	public async Task<Result<MuscleFunction>> Handle(GetMuscleFunctionByIdQuery request,
		CancellationToken cancellationToken)
	{
		var muscleFunction = await dbContext.MuscleFunctions
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return muscleFunction == null
			? Result.Failure<MuscleFunction>(ErrorTypes.NotFound)
			: Result.Success(muscleFunction);
	}
}
