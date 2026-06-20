using Heracles.Application.Features.MuscleFunctions.Commands;
using Heracles.Application.Features.MuscleFunctions.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.MuscleFunctions;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class MuscleFunctionsEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapMuscleFunctionsEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("muscle-functions")
			.RequireAuthorization();

		// GET: api/muscle-functions
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedMuscleFunctionsQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/muscle-functions/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetMuscleFunctionByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/muscle-functions
		group.MapPost("/", async (IMediator mediator, CreateMuscleFunctionRequest request) =>
			{
				var result = await mediator.Send(new CreateMuscleFunctionCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/muscle-functions/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateMuscleFunctionRequest request) =>
			{
				var result = await mediator.Send(new UpdateMuscleFunctionCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/muscle-functions/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveMuscleFunctionCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
