using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using FluentResults;
using Mediator; using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseMuscleGroups.Queries;

/// <summary>
///     Retrieves a <see cref="ExerciseMuscleGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="Mediator" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="ExerciseMuscleGroup" /> group to retrieve.</param>
/// <returns>A <see cref="Result" />.</returns>
public record GetExerciseMuscleGroupByIdQuery(int Id) : IRequest<Result<ExerciseMuscleGroup>>;

/// <summary>
///     Handles the <see cref="GetExerciseMuscleGroupByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetExerciseMuscleGroupByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetExerciseMuscleGroupByIdQuery, Result<ExerciseMuscleGroup>>
{
	public async ValueTask<Result<ExerciseMuscleGroup>> Handle(GetExerciseMuscleGroupByIdQuery request,
		CancellationToken cancellationToken)
	{
		var exerciseMuscleGroup = await dbContext.ExerciseMuscleGroups
			.Include(e => e.Muscle)
			.Include(e => e.Function)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return exerciseMuscleGroup == null
			? Result.Fail<ExerciseMuscleGroup>(ErrorTypes.NotFound)
			: Result.Ok(exerciseMuscleGroup);
	}
}
