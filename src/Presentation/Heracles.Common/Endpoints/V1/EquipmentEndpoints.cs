using Heracles.Application.Features.Equipments.Commands;
using Heracles.Application.Features.Equipments.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.Equipments;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class EquipmentEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapEquipmentsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("equipment")
			.RequireAuthorization();

		// GET: api/equipments
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedEquipmentsQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/equipments/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetEquipmentByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/equipments
		group.MapPost("/", async (IMediator mediator, CreateEquipmentRequest request) =>
			{
				var result = await mediator.Send(new CreateEquipmentCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/equipments/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateEquipmentRequest request) =>
			{
				var result = await mediator.Send(new UpdateEquipmentCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/equipments/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveEquipmentCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
