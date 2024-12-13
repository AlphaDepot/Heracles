using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Equipments;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UserExercises.Queries;

/// <summary>
///     Retrieves a <see cref="UserExercise" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="UserExercise" /> to retrieve.</param>
/// <returns>A <see cref="Result{UserExercise}" />.</returns>
public record UserExercisesByIdQuery(int Id) : IRequest<Result<UserExercise>>;

/// <summary>
///     Handles the <see cref="UserExercisesByIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class UserExercisesByIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<UserExercisesByIdQuery, Result<UserExercise>>
{
	public async Task<Result<UserExercise>> Handle(UserExercisesByIdQuery request, CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (authenticatedUser == null)
		{
			return Result.Failure<UserExercise>(ErrorTypes.Unauthorized);
		}

		var userExercise = await GetUserExerciseByIdAndUserId(request.Id, authenticatedUser, cancellationToken);

		return userExercise == null
			? Result.Failure<UserExercise>(ErrorTypes.NotFound)
			: Result.Success(userExercise);
	}

	private async Task<UserExercise?> GetUserExerciseByIdAndUserId(int id, string userId,
		CancellationToken cancellationToken)
	{
		// Include related entities ExerciseType, MuscleGroups
		var userExercise = await dbContext.UserExercises
			.Include(x => x.ExerciseType)
			.ThenInclude(exerciseType => exerciseType.MuscleGroups)
			.Include(x => x.EquipmentGroup)
			.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

		// If MuscleGroups are included, include related entities Muscle, Function
		if (userExercise?.ExerciseType.MuscleGroups != null)
		{
			await dbContext.Entry(userExercise.ExerciseType)
				.Collection(x => x.MuscleGroups!)
				.Query()
				.Include(x => x.Muscle)
				.Include(x => x.Function)
				.LoadAsync(cancellationToken);
		}

		// if EquipmentGroup is included, include related entity Equipments
		if (userExercise?.EquipmentGroup != null)
		{
			await dbContext.Entry(userExercise.EquipmentGroup)
				.Collection(eg => eg.Equipments ?? new List<Equipment>())
				.LoadAsync(cancellationToken);
		}

		return userExercise;
	}
}
