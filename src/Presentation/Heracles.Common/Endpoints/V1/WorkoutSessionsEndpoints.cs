using Heracles.Application.Features.WorkoutSessions.Commands;
using Heracles.Application.Features.WorkoutSessions.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests.WorkoutSessions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class WorkoutSessionsEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapWorkoutSessionsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("workout-sessions")
			.RequireAuthorization();


		// GET: api/workout-sessions
		group.MapGet("/", async (IMediator mediator) =>
		{
			var result = await mediator.Send(new WorkoutSessionsByUserIdQuery());
			return result.ToApiResponse();
		});

		// GET: api/workout-sessions/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new WorkoutSessionByIdAndUserIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/workout-sessions
		group.MapPost("/", async (IMediator mediator, CreateWorkoutSessionRequest request) =>
		{
			var result = await mediator.Send(new CreateWorkoutSessionCommand(request));
			return result.ToApiResponse();
		});

		// PUT: api/workout-sessions/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateWorkoutSessionRequest request) =>
		{
			var result = await mediator.Send(new UpdateWorkoutSessionCommand(request));
			return result.ToApiResponse();
		});

		// PATCH: api/workout-sessions/{id}/add
		group.MapPatch("/{id:int}/add", async (IMediator mediator, AttachUserExerciseToWorkoutSessionRequest request) =>
		{
			var result = await mediator.Send(new AttachUserExerciseToWorkoutSessionCommand(request));
			return result.ToApiResponse();
		});

		// PATCH: api/workout-sessions/{id}/remove
		group.MapPatch("/{id:int}/remove",
			async (IMediator mediator, DetachUserExerciseToWorkoutSessionRequest request) =>
			{
				var result = await mediator.Send(new DetachUserExerciseToWorkoutSessionCommand(request));
				return result.ToApiResponse();
			});

		// DELETE: api/workout-sessions/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new RemoveWorkoutSessionCommand(id));
			return result.ToApiResponse();
		});

		return app;
	}
}
