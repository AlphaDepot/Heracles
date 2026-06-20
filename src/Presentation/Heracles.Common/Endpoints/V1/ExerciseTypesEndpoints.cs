using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Application.Features.ExerciseTypes.Queries;
using Heracles.Common.Extensions;
using Heracles.Shared.Requests;
using Heracles.Shared.Requests.ExerciseTypes;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Endpoints.V1;

public static class ExerciseTypesEndpoints
{
	private const string Version = "1";

	public static IEndpointRouteBuilder MapExerciseTypesEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app.MapGroup("exercise-types")
			.RequireAuthorization();

		// GET: api/exercise-types
		group.MapGet("/", async (IMediator mediator, [AsParameters] QueryRequest query) =>
		{
			var result = await mediator.Send(new GetPagedExerciseTypesQuery(query));
			return result.ToApiResponse();
		});

		// GET: api/exercise-types/{id}
		group.MapGet("/{id:int}", async (IMediator mediator, int id) =>
		{
			var result = await mediator.Send(new GetExerciseTypeByIdQuery(id));
			return result.ToApiResponse();
		});

		// POST: api/exercise-types
		group.MapPost("/", async (IMediator mediator, CreateExerciseTypeRequest request) =>
			{
				var result = await mediator.Send(new CreateExerciseTypeCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PUT: api/exercise-types/{id}
		group.MapPut("/{id:int}", async (IMediator mediator, UpdateExerciseTypeRequest request) =>
			{
				var result = await mediator.Send(new UpdateExerciseTypeCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PATCH: api/exercise-types/{id}/add
		group.MapPatch("/{id:int}/add", async (IMediator mediator, AttachExerciseMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new AttachExerciseMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// PATCH: api/exercise-types/{id}/remove
		group.MapPatch("/{id:int}/remove", async (IMediator mediator, DetachExerciseMuscleGroupRequest request) =>
			{
				var result = await mediator.Send(new DetachExerciseMuscleGroupCommand(request));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		// DELETE: api/exercise-types/{id}
		group.MapDelete("/{id:int}", async (IMediator mediator, int id) =>
			{
				var result = await mediator.Send(new RemoveExerciseTypeCommand(id));
				return result.ToApiResponse();
			})
			.RequireAuthorization("Admin");

		return app;
	}
}
