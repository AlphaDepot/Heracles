using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ExerciseTypes.Queries;

/// <summary>
///     Retrieves a <see cref="ExerciseType" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="ExerciseType" /> to retrieve.</param>
/// <returns>A <see cref="Result{ExerciseType}" />.</returns>
public record GetExerciseTypeByIdQuery(int Id) : IRequest<Result<ExerciseType>>;

/// <summary>
///     Handles the <see cref="GetExerciseTypeByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
public class GetExerciseTypeByIdQueryHandler(AppDbContext dbContext)
	: IRequestHandler<GetExerciseTypeByIdQuery, Result<ExerciseType>>
{
	public async Task<Result<ExerciseType>> Handle(GetExerciseTypeByIdQuery request,
		CancellationToken cancellationToken)
	{
		var exerciseType = await dbContext.ExerciseTypes
			.Include(x => x.MuscleGroups!)
			.ThenInclude(x => x.Muscle)
			.Include(x => x.MuscleGroups)!
			.ThenInclude(x => x.Function)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

		return exerciseType == null
			? Result.Failure<ExerciseType>(ErrorTypes.NotFound)
			: Result.Success(exerciseType);
	}
}
