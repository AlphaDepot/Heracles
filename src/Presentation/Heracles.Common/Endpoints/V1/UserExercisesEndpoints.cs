using Heracles.Application.Features.UserExercises.Commands;
using Heracles.Application.Features.UserExercises.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.UserExercises;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class UserExercisesEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapUserExercisesEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("user-exercises")
			.RequireAuthorization();

		// GET: api/user-exercises
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new UserPagedExercisesByUserIdQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/user-exercises/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new UserExercisesByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/user-exercises
		group.MapPost("/", async (IMediator mediator, CreateUserExerciseRequest request) =>
		{
			var result = await mediator.Send(new CreateUserExerciseCommand(request));
			return result.ToApiResponse();
		});

		// PUT: api/user-exercises/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateUserExerciseRequest request) =>
		{
			var result = await mediator.Send(new UpdateUserExerciseCommand(request));
			return result.ToApiResponse();
		});

		// DELETE: api/user-exercises/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new RemoveUserExerciseCommand(id));
			return result.ToApiResponse();
		});

		return app;
	}
}
