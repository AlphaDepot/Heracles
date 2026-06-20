using Heracles.Application.Features.Users.Commands;
using Heracles.Application.Features.Users.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests.Users;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class UsersEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("users")
			.RequireAuthorization();

		// GET: api/users/{userId}
		group.MapGet("/{userId}", async (IMediator mediator, string userId) =>
		{
			var result = await mediator.Send(new GetUserByUserIdQuery(userId));
			return result.ToApiResponse();
		});

		// POST: api/users
		group.MapPost("/", async (IMediator mediator, CreateUserRequest request) =>
			{
				var result = await mediator.Send(new CreateUserCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/users/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateUserRequest request) =>
		{
			var result = await mediator.Send(new UpdateUserCommand(request));
			return result.ToApiResponse();
		});

		// PATCH: api/users
		group.MapPatch("/", async (IMediator mediator, CreateOrUpdateRequest request) =>
		{
			var result = await mediator.Send(new CreateOrUpdateCommand(request));
			return result.ToApiResponse();
		});

		// DELETE: api/users/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveUserCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
