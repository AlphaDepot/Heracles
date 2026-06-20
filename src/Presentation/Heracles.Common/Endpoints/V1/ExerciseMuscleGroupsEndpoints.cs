using Heracles.Application.Features.ExerciseMuscleGroups.Commands;
using Heracles.Application.Features.ExerciseMuscleGroups.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.ExerciseMuscleGroups;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class ExerciseMuscleGroupsEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapExerciseMuscleGroupsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("exercise-muscle-groups")
			.RequireAuthorization();

		// GET: api/exercise-muscle-groups
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedExerciseMuscleGroupQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/exercise-muscle-groups/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetExerciseMuscleGroupByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/exercise-muscle-groups
		group.MapPost("/", async (IMediator mediator, CreateExerciseMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new CreateExerciseMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/exercise-muscle-groups/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateExerciseMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new UpdateExerciseMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/exercise-muscle-groups/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveExerciseMuscleGroupCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
