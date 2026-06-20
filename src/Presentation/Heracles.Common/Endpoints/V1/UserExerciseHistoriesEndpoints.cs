using Heracles.Application.Features.UserExerciseHistories.Commands;
using Heracles.Application.Features.UserExerciseHistories.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests.UserExerciseHistories;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class UserExerciseHistoriesEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapUserExerciseHistoriesEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("user-exercise-histories")
			.RequireAuthorization();

		// GET: api/user-exercise-histories/by-user-exercise/{userExerciseId}
		group.MapGet("by-user-exercise/{userExerciseId:int}", async (IMediator mediator, int userExerciseId) =>
		{
			var result = await mediator.Send(new UserExerciseHistoriesByUserExerciseIdQuery(userExerciseId));
			return result.ToApiResponse();
		});

		// GET: api/user-exercise-histories/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new UserExerciseHistoryByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/user-exercise-histories
		group.MapPost("/", async (IMediator mediator, CreateUserExerciseHistoryRequest request) =>
		{
			var result = await mediator.Send(new CreateUserExerciseHistoryCommand(request));
			return result.ToApiResponse();
		});

		// PUT: api/user-exercise-histories/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateUserExerciseHistoryRequest request) =>
		{
			var result = await mediator.Send(new UpdateUserExerciseHistoryCommand(request));
			return result.ToApiResponse();
		});

		// DELETE: api/user-exercise-histories/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new RemoveUserExerciseHistoryCommand(id));
			return result.ToApiResponse();
		});

		return app;
	}
}
