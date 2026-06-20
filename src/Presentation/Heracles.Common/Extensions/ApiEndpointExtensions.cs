using Heracles.Common.Endpoints.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Heracles.Common.Extensions;

public static class ApiEndpointExtensions
{
	public static WebApplication MapApiEndpoints(
		this WebApplication app)
	{
		var api = app.MapGroup("/api");

		api.MapV1Endpoints();
		// api.MapV2Endpoints();

		return app;
	}

	private static RouteGroupBuilder MapV1Endpoints(
		this RouteGroupBuilder api)
	{
		var v1 = api.MapGroup("/v1").WithTags("V1");
		;

		v1.MapEquipmentGroupsEndpoints();
		v1.MapEquipmentsEndpoints();
		v1.MapExerciseMuscleGroupsEndpoints();
		v1.MapExerciseTypesEndpoints();
		v1.MapMuscleFunctionsEndpoints();
		v1.MapMuscleGroupsEndpoints();
		v1.MapUserExerciseHistoriesEndpoints();
		v1.MapUserExercisesEndpoints();
		v1.MapUsersEndpoints();
		v1.MapWorkoutSessionsEndpoints();

		return v1;
	}
}
