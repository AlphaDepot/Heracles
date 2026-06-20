using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Application.Features.EquipmentGroups.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.EquipmentGroups;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class EquipmentGroupsEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapEquipmentGroupsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("equipment-groups")
			.RequireAuthorization();

		// GET: api/equipment-groups
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedEquipmentGroupsQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/equipment-groups/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetEquipmentGroupByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/equipment-groups
		group.MapPost("/", async (IMediator mediator, CreateEquipmentGroupRequest request) =>
			{
				var result = await mediator.Send(new CreateEquipmentGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/equipment-groups/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateEquipmentGroupRequest request) =>
			{
				var result = await mediator.Send(new UpdateEquipmentGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PATCH: api/equipment-groups/{id}/add
		group.MapPatch("/{id:int}/add", async (IMediator mediator, AttachEquipmentGroupRequest groupRequest) =>
			{
				var result = await mediator.Send(new AttachEquipmentCommand(groupRequest));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PATCH: api/equipment-groups/{id}/remove
		group.MapPatch("/{id:int}/remove", async (IMediator mediator, DetachEquipmentGroupRequest groupRequest) =>
			{
				var result = await mediator.Send(new DetachEquipmentGroupCommand(groupRequest));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/equipmentgroups/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveEquipmentGroupCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
