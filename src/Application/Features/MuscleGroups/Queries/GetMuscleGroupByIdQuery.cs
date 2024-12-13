using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MuscleGroups.Queries;

/// <summary>
///     Retrieves a <see cref="MuscleGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="MuscleGroup" /> to retrieve.</param>
/// <returns>A <see cref="Result{MuscleGroup}" />.</returns>
public record GetMuscleGroupByIdQuery(int Id) : IRequest<Result<MuscleGroup>>;

/// <summary>
///     Handles the <see cref="GetMuscleGroupByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetMuscleGroupByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetMuscleGroupByIdQuery, Result<MuscleGroup>>
{
	public async Task<Result<MuscleGroup>> Handle(GetMuscleGroupByIdQuery request, CancellationToken cancellationToken)
	{
		var muscleGroup = await dbContext.MuscleGroups
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return muscleGroup == null
			? Result.Failure<MuscleGroup>(ErrorTypes.NotFound)
			: Result.Success(muscleGroup);
	}
}
