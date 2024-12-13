using System.Security.Claims;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.WorkoutSessions.Queries;

/// <summary>
///     Retrieves a <see cref="WorkoutSession" /> by id.
/// </summary>
/// <remarks>
///     Utilizes <see cref="IRequestHandler{TRequest,TResponse}" /> from <see cref="MediatR" /> to process the command.
/// </remarks>
/// <param name="Id">The id of the <see cref="WorkoutSession" /> to retrieve.</param>
/// <returns>A <see cref="Result{WorkoutSession}" />.</returns>
public record WorkoutSessionByIdAndUserIdQuery(int Id) : IRequest<Result<WorkoutSession>>;

/// <summary>
///     Handles the <see cref="Application.Features.WorkoutSessions.Queries.WorkoutSessionByIdAndUserIdQueryHandler" />.
/// </summary>
/// <param name="dbContext">The <see cref="AppDbContext" />.</param>
/// <param name="contextAccessor">The <see cref="IHttpContextAccessor" />.</param>
public class WorkoutSessionByIdAndUserIdQueryHandler(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
	: IRequestHandler<WorkoutSessionByIdAndUserIdQuery, Result<WorkoutSession>>
{
	public async Task<Result<WorkoutSession>> Handle(WorkoutSessionByIdAndUserIdQuery request,
		CancellationToken cancellationToken)
	{
		var authenticatedUser = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (authenticatedUser == null)
		{
			return Result.Failure<WorkoutSession>(ErrorTypes.Unauthorized);
		}

		var session = await dbContext.WorkoutSessions
			.Include(x => x.UserExercises)
			.Where(x => x.UserId == authenticatedUser)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);


		return session == null
			? Result.Failure<WorkoutSession>(ErrorTypes.NotFound)
			: Result.Success(session);
	}
}
