using Heracles.Application.Features.MuscleGroups.Commands;
using Heracles.Application.Features.MuscleGroups.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.MuscleGroups;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class MuscleGroupsEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapMuscleGroupsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("muscle-groups")
			.RequireAuthorization();

		// GET: api/muscle-groups
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedMuscleGroupsQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/muscle-groups/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetMuscleGroupByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/muscle-groups
		group.MapPost("/", async (IMediator mediator, CreateMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new CreateMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/muscle-groups/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new UpdateMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/muscle-groups/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveMuscleGroupCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
