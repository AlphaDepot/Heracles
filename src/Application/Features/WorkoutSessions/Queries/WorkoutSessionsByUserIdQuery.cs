using System.Security.Claims;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Queries;

/// <summary>
///     Retrieves a list of <see cref="WorkoutSession" />s associated with the currently authenticated user.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <returns>A <see cref="Result{List}" />.</returns>
public record WorkoutSessionsByUserIdQuery : IRequest<Result<List<WorkoutSession>>>;

/// <summary>
///     Handles the <see cref="WorkoutSessionsByUserIdQuery" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class WorkoutSessionsByUserIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<WorkoutSessionsByUserIdQuery, Result<List<WorkoutSession>>>
{
	public async Task<Result<List<WorkoutSession>>> Handle(WorkoutSessionsByUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		var sessions = await dbContext.WorkoutSessions
			.Include(x => x.UserExercises)
			.Where(x => x.UserId == authenticatedUser)
			.OrderBy(x => x.DayOfWeek)
			.ThenBy(x => x.SortOrder).ToListAsync(cancellationToken);

		return Result.Success(sessions);
	}
}
