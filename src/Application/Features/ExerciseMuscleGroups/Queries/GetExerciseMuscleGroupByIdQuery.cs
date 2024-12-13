using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseMuscleGroups.Queries;

/// <summary>
///     Retrieves a <see cref="ExerciseMuscleGroup" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="ExerciseMuscleGroup" /> group to retrieve.</param>
/// <returns>A <see cref="Result{ExerciseMuscleGroup}" />.</returns>
public record GetExerciseMuscleGroupByIdQuery(int Id) : IRequest<Result<ExerciseMuscleGroup>>;

/// <summary>
///     Handles the <see cref="GetExerciseMuscleGroupByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetExerciseMuscleGroupByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetExerciseMuscleGroupByIdQuery, Result<ExerciseMuscleGroup>>
{
	public async Task<Result<ExerciseMuscleGroup>> Handle(GetExerciseMuscleGroupByIdQuery request,
		CancellationToken cancellationToken)
	{
		var exerciseMuscleGroup = await dbContext.ExerciseMuscleGroups
			.Include(e => e.Muscle)
			.Include(e => e.Function)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return exerciseMuscleGroup == null
			? Result.Failure<ExerciseMuscleGroup>(ErrorTypes.NotFound)
			: Result.Success(exerciseMuscleGroup);
	}
}
